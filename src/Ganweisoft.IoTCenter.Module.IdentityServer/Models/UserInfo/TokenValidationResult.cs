using System.Collections.Generic;
using System.Security.Claims;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;

public class TokenValidationResult
{
    /// <summary>
    /// Gets or sets the claims.
    /// </summary>
    /// <value>
    /// The claims.
    /// </value>
    public IEnumerable<Claim> Claims { get; set; }
        
    /// <summary>
    /// Gets or sets the JWT.
    /// </summary>
    /// <value>
    /// The JWT.
    /// </value>
    public string Jwt { get; set; }

    /// <summary>
    /// Gets or sets the IsValid.
    /// </summary>
    /// <value>
    /// The IsValid.
    /// </value>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the client.
    /// </summary>
    /// <value>
    /// The client.
    /// </value>
    public string AppId { get; set; }
}