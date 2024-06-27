using System.Security.Claims;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Generator;

public class AuthorizeInteractionResponseGenerator : IAuthorizeInteractionResponseGenerator
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// The clock
    /// </summary>
    protected readonly ISystemClock Clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeInteractionResponseGenerator"/> class.
    /// </summary>
    /// <param name="clock">The clock.</param>
    /// <param name="logger">The logger.</param>
    public AuthorizeInteractionResponseGenerator(
        ISystemClock clock,
        ILogger<AuthorizeInteractionResponseGenerator> logger)
    {
        Clock = clock;
        Logger = logger;
    }

    /// <summary>
    /// Processes the interaction logic.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public virtual async Task<bool> ProcessInteractionAsync(ValidatedAuthorizeRequest request)
    {
        Logger.LogDebug("ProcessInteractionAsync");

        return await ProcessLoginAsync(request);
    }

    /// <summary>
    /// Processes the login logic.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    protected virtual async Task<bool> ProcessLoginAsync(ValidatedAuthorizeRequest request)
    {
        // check client's user SSO timeout
        var claimsIdentity = request.Subject.Identity as ClaimsIdentity;

        var authTimeClaim = claimsIdentity?.FindFirst(ClaimTypes.Expired);
        if (authTimeClaim == null || authTimeClaim.ValueType != ClaimValueTypes.Integer64)
        {
            return false;
        }
        
        var authTimeEpoch = long.Parse(authTimeClaim.Value);
        var nowEpoch = Clock.UtcNow.ToUnixTimeSeconds();

        var diff = nowEpoch - authTimeEpoch;
        if (diff > IdentityServerConstant.UserSsoLifetime)
        {
            Logger.LogInformation("Showing login: User's auth session duration: {sessionDuration} exceeds client's user SSO lifetime: {userSsoLifetime}.", diff, IdentityServerConstant.UserSsoLifetime);
            return false;
        }

        return await Task.FromResult(true);
    }
}