using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Microsoft.Extensions.Logging;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Generator;

public class UserInfoResponseGenerator : IUserInfoResponseGenerator
{
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserInfoResponseGenerator"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UserInfoResponseGenerator(ILogger<UserInfoResponseGenerator> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Creates the response.
    /// </summary>
    /// <param name="validationResult">The userinfo request validation result.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">Profile service returned incorrect subject value</exception>
    public virtual async Task<UserInfoResponse> ProcessAsync(UserInfoRequestValidationResult validationResult)
    {
        Logger.LogDebug("Creating userinfo response");

        var claimsIdentity = validationResult.Subject.Identity as ClaimsIdentity;
        if (claimsIdentity == null)
        {
            return new UserInfoResponse();
        }
        
        var authNameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
        var authTimeClaim = claimsIdentity.FindFirst(ClaimTypes.Expired);
        var scopeClaims = claimsIdentity.FindAll(IdentityServerConstant.GwClaimTypesScope);
        // call profile service
        var result = new UserInfoResponse
        {
            IsLogin = true,
            LoginName = authNameClaim?.Value,
            LoginTime = authTimeClaim?.Value,
            LoginScope = scopeClaims.Select(c => c.Value)
        };

        return await Task.FromResult(result);
    }
}