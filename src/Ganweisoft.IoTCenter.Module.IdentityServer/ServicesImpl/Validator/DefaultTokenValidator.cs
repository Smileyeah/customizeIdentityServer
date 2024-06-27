using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Resource;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TokenValidationResult = Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo.TokenValidationResult;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Validator;

public class DefaultTokenValidator : ITokenValidator
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;
    
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly IConfiguration Configuration;

    /// <summary>
    /// The memeryCache service
    /// </summary>
    protected readonly IMemoryCache MemCache;
    
    protected readonly IResourceValidator ResourceValidator;
    
    protected readonly IAuthorizeInteractionResponseGenerator InteractionGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeInteractionResponseGenerator"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The logger.</param>
    /// <param name="memCache">The logger.</param>
    /// <param name="resourceValidator"></param>
    /// <param name="interactionGenerator"></param>
    public DefaultTokenValidator(
        IConfiguration configuration,
        ILogger<DefaultTokenValidator> logger,
        IMemoryCache memCache, IResourceValidator resourceValidator, 
        IAuthorizeInteractionResponseGenerator interactionGenerator)
    {
        Logger = logger;
        Configuration = configuration;
        MemCache = memCache;
        ResourceValidator = resourceValidator;
        InteractionGenerator = interactionGenerator;
    }
    
    public async Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null)
    {
        var handler = new JwtSecurityTokenHandler();
        handler.InboundClaimTypeMap.Clear();

        var parameters = new TokenValidationParameters
        {
            ValidIssuer = Configuration["Authentication:JwtBearer:Issuer"],
            IssuerSigningKeys = new SecurityKey[]{new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:JwtBearer:SecurityKey"]))},
            ValidateLifetime = true,
            ValidAudience = Configuration["Authentication:JwtBearer:Audience"]
        };
        
        var id = handler.ValidateToken(token, parameters, out _);
        var jwtId = id.FindFirst(IdentityServerConstant.GwClientClaim);
        if (string.IsNullOrEmpty(jwtId?.Value))
        {
            return new TokenValidationResult { IsValid = false };
        }

        // get cache Token Request Validation Result
        var userId = id.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!MemCache.TryGetValue<TokenRequestValidationResult>($"{IdentityServerConstant.GrantType}:{userId}",
                out var tokenValidationResult))
        {
            return new TokenValidationResult { IsValid = false };
        }
        
        // check expected scope(s)
        var enableExpectedScope = Configuration.GetSection("IdentityServer:EnableExpectedScope").Get<bool>();
        if (enableExpectedScope)
        {
            var expectedScopeStr = Configuration.GetSection("IdentityServer:ExpectedScope").Get<string[]>();
            var scope = id.FindFirst(c =>
                c.Type == IdentityServerConstant.GwClaimTypesScope &&
                (c.Value.Equals(expectedScope, StringComparison.OrdinalIgnoreCase) ||
                 expectedScopeStr.Any(e => e.Equals(c.Value, StringComparison.OrdinalIgnoreCase))));
            if (scope != null)
            {
                Logger.LogError($"Token validate for expected scope {expectedScope} success");
                
                return new TokenValidationResult
                {
                    Claims = id.Claims,
                    Jwt = token,
                    IsValid = true,
                    AppId = jwtId.Value
                };
            }
        }

        var validate = await ResourceValidator.ValidateResourceAsync(new ResourceValidationRequest
        {
            UserId = userId,
            ResourceName = tokenValidationResult.ValidatedRequest.AppId
        });

        if (!validate.IsValid)
        {
            return new TokenValidationResult { IsValid = false };
        }

        // validate token life time expired
        var timeValidate = await InteractionGenerator.ProcessInteractionAsync(tokenValidationResult.ValidatedRequest);
        if (!timeValidate)
        {
            return new TokenValidationResult { IsValid = false };
        }
        
        return new TokenValidationResult
        {
            Claims = id.Claims,
            Jwt = token,
            IsValid = true,
            AppId = jwtId.Value
        };
    }
}