using System.Collections.Generic;
using System.Security.Claims;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

/// <summary>
/// Base class for a validate authorize or token request
/// </summary>
public class ValidatedRequest
{
    /// <summary>
    /// Gets or sets the effective access token lifetime for the current request.
    /// This value is initally read from the client configuration but can be modified in the request pipeline
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 7200;
    

    /// <summary>
    /// Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
    /// </summary>
    public int AuthorizationCodeLifetime { get; set; } = 300;

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public ClaimsPrincipal Subject { get; set; }

    /// <summary>
    /// Gets or sets the client ID that should be used for the current request (this is useful for token exchange scenarios)
    /// </summary>
    /// <value>
    /// The client ID
    /// </value>
    public string AppId { get; set; }
}

public class ValidatedAuthorizeRequest : ValidatedRequest
{
    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    /// <value>
    /// The redirect URI.
    /// </value>
    public string RedirectUri { get; set; }

    /// <summary>
    /// Gets the description the user assigned to the device being authorized.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the nonce.
    /// </summary>
    /// <value>
    /// The nonce.
    /// </value>
    public string Nonce { get; set; }

    /// <summary>
    /// Gets or sets the requested scopes.
    /// </summary>
    /// <value>
    /// The requested scopes.
    /// </value>
    public List<string> RequestedScopes { get; set; }

    public string Scope { get; set; }
}