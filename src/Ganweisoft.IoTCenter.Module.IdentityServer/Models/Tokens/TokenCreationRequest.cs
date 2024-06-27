using System.Security.Claims;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;

public class TokenCreationRequest
{
    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public ClaimsPrincipal Subject { get; set; }

    /// <summary>
    /// Gets or sets the access token to hash.
    /// </summary>
    /// <value>
    /// The access token to hash.
    /// </value>
    public string AccessTokenToHash { get; set; }

    /// <summary>
    /// Gets or sets the authorization code to hash.
    /// </summary>
    /// <value>
    /// The authorization code to hash.
    /// </value>
    public string AuthorizationCodeToHash { get; set; }

    /// <summary>
    /// Gets or sets pre-hashed state
    /// </summary>
    /// <value>
    /// The pre-hashed state
    /// </value>
    public string StateHash { get; set; }

    /// <summary>
    /// Gets or sets the nonce.
    /// </summary>
    /// <value>
    /// The nonce.
    /// </value>
    public string Nonce { get; set; }

    /// <summary>
    /// Gets the description the user assigned to the device being authorized.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Unique ID of the client
    /// </summary>
    /// <value>
    /// The Client Id.
    /// </value>
    public string AppId { get; set; }

    /// <summary>
    /// Gets or sets the effective access token lifetime for the current request.
    /// This value is initally read from the client configuration but can be modified in the request pipeline
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 7200;
}