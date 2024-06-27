namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

public class AuthorizeResponse
{
    public ValidatedAuthorizeRequest Request { get; set; }
    public string RedirectUri => Request?.RedirectUri;
    public string State => Request?.State;

    public string IdentityToken { get; set; }
    public string AccessToken { get; set; }
    public int AccessTokenLifetime { get; set; }
    public string Code { get; set; }
    public string SessionState => Request?.State;
}