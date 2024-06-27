using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;

public class UserInfoResponse
{
    /// <summary>
    /// 是否登录
    /// </summary>
    public bool IsLogin { get; set; }
    
    /// <summary>
    /// 登录时间
    /// </summary>
    public string? LoginTime { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? LoginName { get; set; }
    
    /// <summary>
    /// scope
    /// </summary>
    public IEnumerable<string> LoginScope { get; set; }
}