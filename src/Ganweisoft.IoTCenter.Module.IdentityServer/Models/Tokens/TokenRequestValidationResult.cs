using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;

/// <summary>
/// 
/// </summary>
public class TokenRequestValidationResult
{
    /// <summary>
    /// Gets the validated request.
    /// </summary>
    /// <value>
    /// The validated request.
    /// </value>
    public ValidatedTokenRequest ValidatedRequest { get; init; }
}

public class ValidatedTokenRequest : ValidatedAuthorizeRequest
{
    /// <summary>
    /// Gets or sets the authorization code.
    /// </summary>
    /// <value>
    /// The authorization code.
    /// </value>
    public AuthorizationCode AuthorizationCode { get; set; }
}