using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.UserInfo;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;

public interface ITokenValidator
{
    /// <summary>
    /// Validates an access token.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <param name="expectedScope">The expected scope.</param>
    /// <returns></returns>
    Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null);
}