using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;

/// <summary>
/// Interface for determining if user must login or consent when making requests to the authorization endpoint.
/// </summary>
public interface IAuthorizeInteractionResponseGenerator
{
    /// <summary>
    /// Processes the interaction logic.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    Task<bool> ProcessInteractionAsync(ValidatedAuthorizeRequest request);
}