using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Connect;

public class ConnectCodeRequest
{
    /// <summary>
    /// 负载名
    /// </summary>
    [Required]
    public string AppId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Nonce { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string State { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string RedirectUri { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Scope { get; set; }
}