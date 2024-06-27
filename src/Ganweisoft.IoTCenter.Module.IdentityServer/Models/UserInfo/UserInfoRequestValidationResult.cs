using System.Security.Claims;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;

public class UserInfoRequestValidationResult
{
    /// <summary>
    /// Gets or sets the token validation result.
    /// </summary>
    /// <value>
    /// The token validation result.
    /// </value>
    public TokenValidationResult TokenValidationResult { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public ClaimsPrincipal Subject { get; set; }
}