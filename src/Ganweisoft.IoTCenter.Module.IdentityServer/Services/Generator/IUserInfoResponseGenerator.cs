using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;

/// <summary>
/// Interface for the userinfo response generator
/// </summary>
public interface IUserInfoResponseGenerator
{
    /// <summary>
    /// Creates the response.
    /// </summary>
    /// <param name="validationResult">The userinfo request validation result.</param>
    /// <returns></returns>
    Task<UserInfoResponse> ProcessAsync(UserInfoRequestValidationResult validationResult);
}