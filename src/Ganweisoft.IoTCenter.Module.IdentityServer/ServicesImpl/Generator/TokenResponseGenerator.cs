using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Resource;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;
using IoTCenterWebApi;
using IoTCenterWebApi.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Generator;

public class TokenResponseGenerator : ITokenResponseGenerator
{
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// The token service
    /// </summary>
    protected readonly ITokenService TokenService;

    /// <summary>
    /// The token service
    /// </summary>
    protected readonly IMemoryCache MemCache;
    
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly IConfiguration Configuration;

    /// <summary>
    /// The token service
    /// </summary>
    protected readonly IResourceValidator ResourceValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenResponseGenerator" /> class.
    /// </summary>
    /// <param name="tokenService">The token service.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memCache">The logger.</param>
    /// <param name="resourceValidator">The logger.</param>
    /// <param name="configuration"></param>
    public TokenResponseGenerator(
        ITokenService tokenService, 
        ILogger<TokenResponseGenerator> logger, 
        IMemoryCache memCache, 
        IResourceValidator resourceValidator, 
        IConfiguration configuration)
    {
        TokenService = tokenService;
        Logger = logger;
        MemCache = memCache;
        ResourceValidator = resourceValidator;
        Configuration = configuration;
    }

    /// <summary>
    /// Processes the response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public virtual async Task<TokenResponse> ProcessAsync(TokenRequestValidationResult request)
    {
        return await ProcessClientCredentialsRequestAsync(request);
    }

    /// <summary>
    /// Creates the response for an authorization code request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">Client does not exist anymore.</exception>
    protected virtual async Task<TokenResponse> ProcessClientCredentialsRequestAsync(TokenRequestValidationResult request)
    {
        Logger.LogDebug("Creating response for client credentials request");

        return await ProcessTokenRequestAsync(request);
    }

    /// <summary>
    /// Creates the access/refresh token.
    /// </summary>
    /// <param name="validationResult">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">Client does not exist anymore.</exception>
    protected virtual async Task<TokenResponse> ProcessTokenRequestAsync(TokenRequestValidationResult validationResult)
    {
        // check expected scope(s)
        var enableExpectedScope = Configuration.GetSection("IdentityServer:EnableExpectedScope").Get<bool>();
        if (enableExpectedScope)
        {
            var expectedScopeStr = Configuration.GetSection("IdentityServer:ExpectedScope").Get<string[]>();
            var scope = validationResult.ValidatedRequest.AuthorizationCode.Subject.FindFirst(c =>
                c.Type == IdentityServerConstant.GwClaimTypesScope &&
                 expectedScopeStr.Any(e => e.Equals(c.Value, StringComparison.OrdinalIgnoreCase)));
            if (scope == null && validationResult.ValidatedRequest.AppId != validationResult.ValidatedRequest.AuthorizationCode.AppId)
            {
                Logger.LogError($"Client is trying to use a code from a different client.{validationResult.ValidatedRequest.AppId} : {validationResult.ValidatedRequest.AuthorizationCode.AppId}");
                throw new InvalidOperationException("Client is trying to use a code from a different client.");
            }
        }
        
        var accessToken = await CreateAccessTokenAsync(validationResult.ValidatedRequest);
        var response = new TokenResponse
        {
            AccessToken = accessToken,
            AccessTokenLifetime = validationResult.ValidatedRequest.AccessTokenLifetime,
            State = validationResult.ValidatedRequest.AuthorizationCode.StateHash,
            AppId = validationResult.ValidatedRequest.AppId
        };

        // 缓存的登录token
        var userId = validationResult.ValidatedRequest.AuthorizationCode.Subject.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException($"The ClaimsPrincipal of AuthorizationCode:{validationResult.ValidatedRequest.AppId} is not find.");
        }

        // 存储token和client的绑定关系。check的时候再对比
        MemCache.Set($"{IdentityServerConstant.GrantType}:{userId}", validationResult,
            TimeSpan.FromSeconds(IdentityServerConstant.UserSsoLifetime));

        return response;
    }

    /// <summary>
    /// Creates the access/refresh token.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">Client does not exist anymore.</exception>
    protected virtual async Task<string> CreateAccessTokenAsync(ValidatedTokenRequest request)
    {
        var enableExpectedScope = Configuration.GetSection("IdentityServer:EnableExpectedScope").Get<bool>();
        if (enableExpectedScope)
        {
            var expectedScopeStr = Configuration.GetSection("IdentityServer:ExpectedScope").Get<string[]>();
            var scope = request.AuthorizationCode.Subject.FindFirst(c =>
                c.Type == IdentityServerConstant.GwClaimTypesScope &&
                expectedScopeStr.Any(e => e.Equals(c.Value, StringComparison.OrdinalIgnoreCase)));
            if (scope == null && !string.IsNullOrEmpty(request.AppId))
            {
                // load the client that belongs to the authorization code
                var resourceValidation = await ResourceValidator.ValidateResourceAsync(new ResourceValidationRequest()
                {
                    ResourceName = request.AppId,
                    Scope = request.Scope,
                    UserId = request.Subject.FindFirst(c => c.Type == ClaimTypes.Name)?.Value
                });
                if (resourceValidation is null || !resourceValidation.IsValid)
                {
                    throw new InvalidOperationException("PodResource does not exist anymore.");
                }
            }
        }

        var tokenRequest = new TokenCreationRequest
        {
            AppId = request.AppId,
            Nonce = request.AuthorizationCode.Nonce,
            Subject = request.AuthorizationCode.Subject,
            Description = request.AuthorizationCode.Description,
            StateHash = request.AuthorizationCode.StateHash
        };

        var at = await TokenService.CreateAccessTokenAsync(tokenRequest);
        var accessToken = this.GetJwtToken(at);

        return accessToken;
    }

    private string GetJwtToken(Token token)
    {
        var expired = token.CreationTime.AddSeconds(token.Lifetime);

        string securityKey = EncDecHelper.AdvanceDecrypt(Configuration["Authentication:JwtBearer:SecurityKey"], out string msg);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenKey = new JwtSecurityToken(
            issuer: Configuration["Authentication:JwtBearer:Issuer"],
            audience: Configuration["Authentication:JwtBearer:Audience"],
            claims: token.Claims,
            expires: expired,
            signingCredentials: credentials);

        try
        {
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(tokenKey);
            return tokenStr;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return default;
        }
    }
}