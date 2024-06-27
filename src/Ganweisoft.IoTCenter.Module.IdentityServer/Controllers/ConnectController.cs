using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Connect;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Resource;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Store;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Utils;
using IoTCenter.Utilities;
using IoTCenterWebApi.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Controllers;

[ApiController]
[Route("api/idsvr/[controller]/[action]")]
public class ConnectController : DefaultController
{
        
    private readonly IAuthorizeResponseGenerator _authorizeGenerator;
    private readonly ITokenResponseGenerator _tokenGenerator;
    private readonly IUserInfoResponseGenerator _userInfoGenerator;
    
    private readonly ITokenValidator _tokenValidator;
    private readonly IResourceValidator _resourceValidator;
    private readonly IAuthorizationCodeStore _codeStore;
    private readonly ISystemClock _clock;
    private readonly Session _session;
    
    public ConnectController(IAuthorizeResponseGenerator authorizeGenerator,
        ITokenResponseGenerator tokenGenerator,
        IUserInfoResponseGenerator userInfoGenerator,
        ITokenValidator tokenValidator,
        IResourceValidator resourceValidator, 
        IAuthorizationCodeStore codeStore, 
        ISystemClock clock,
        Session session)
    {
        this._authorizeGenerator = authorizeGenerator;
        this._userInfoGenerator = userInfoGenerator;
        this._tokenGenerator = tokenGenerator;

        this._tokenValidator = tokenValidator;
        this._resourceValidator = resourceValidator;
        this._codeStore = codeStore;
        this._clock = clock;
        this._session = session;
    }
    
    /// <summary>
    /// 获取跳转Code，需要敢为云登录验证，有效时间为5分钟
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperateResult<string>> Connect([FromQuery] ConnectCodeRequest request)
    {
        var validate = await _resourceValidator.ValidateResourceAsync(new ResourceValidationRequest
        {
            UserId = this._session.UserId.ToString(),
            ResourceName = request.AppId,
            Scope = request.Scope
        });

        if (!validate.IsValid)
        {
            return OperateResult.Failed<string>(validate.ErrorMessage);
        }
        
        if (string.IsNullOrWhiteSpace(request.Nonce))
        {
            request.Nonce = CryptoRandom.CreateUniqueId();
        }

        var subject = new ClaimsIdentity(HttpContext.User.Claims);
        if (!string.IsNullOrWhiteSpace(request.Scope))
        {
            subject.AddClaim(new Claim(IdentityServerConstant.GwClaimTypesScope, request.Scope));
        }
        
        var response = await this._authorizeGenerator.CreateResponseAsync(new ValidatedAuthorizeRequest
        {
            AppId = request.AppId,
            RedirectUri = request.RedirectUri,
            Nonce = request.Nonce,
            Description = request.Description,
            State = request.State,
            Subject = new ClaimsPrincipal(subject),
            Scope = request.Scope,
        });

        return OperateResult.Successed(response.Code);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<OperateResult<TokenResponse>> Token([FromQuery] ConnectTokenRequest request)
    {
        try
        {
            var authCode = await _codeStore.GetAuthorizationCodeAsync(request.Code);
            if (authCode == default)
            {
                return OperateResult.Failed<TokenResponse>("Code错误");
            }

            if (authCode.CreationTime.AddSeconds(authCode.Lifetime) < _clock.UtcNow)
            {
                return OperateResult.Failed<TokenResponse>("Code已过期");
            }
        
            var response = await this._tokenGenerator.ProcessAsync(new TokenRequestValidationResult
            {
                ValidatedRequest = new ValidatedTokenRequest
                {
                    AuthorizationCode = authCode,
                    AppId = request.AppId
                }
            });
        
            return OperateResult.Successed(response);
        }
        finally
        {
            await _codeStore.RemoveAuthorizationCodeAsync(request.Code);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<OperateResult<bool>> Authorize([FromQuery] AuthorizeTokenRequest request)
    {
        var token = await this._tokenValidator.ValidateAccessTokenAsync(request.AccessToken);
        
        return OperateResult.Successed(token.IsValid);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<OperateResult<UserInfoResponse>> UserInfo([FromQuery] ConnectUserRequest request)
    {
        var token = await this._tokenValidator.ValidateAccessTokenAsync(request.AccessToken);
        if (!token.IsValid)
        {
            return OperateResult.Failed<UserInfoResponse>("Token验证失败");
        }

        var response = await this._userInfoGenerator.ProcessAsync(new UserInfoRequestValidationResult()
        {
            TokenValidationResult = token,
            Subject = new ClaimsPrincipal(new ClaimsIdentity(token.Claims))
        });
        
        return OperateResult.Successed(response);
    }
}