using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;

/// <summary>
/// Interface the token response generator
/// </summary>
public interface ITokenResponseGenerator
{
    /// <summary>
    /// Processes the response.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns></returns>
    Task<TokenResponse> ProcessAsync(TokenRequestValidationResult validationResult);
}