namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

public class TokenResponse
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    /// <value>
    /// The access token.
    /// </value>
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the access token lifetime.
    /// </summary>
    /// <value>
    /// The access token lifetime.
    /// </value>
    public int AccessTokenLifetime { get; set; }
    
    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the client id.
    /// </summary>
    /// <value>
    /// The client id.
    /// </value>
    public string AppId { get; set; }
}