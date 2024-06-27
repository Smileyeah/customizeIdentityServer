namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Connect;

public class ConnectTokenRequest
{
    /// <summary>
    /// 
    /// </summary>
    public string AppId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Secret { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string GrantType { get; set; }
}