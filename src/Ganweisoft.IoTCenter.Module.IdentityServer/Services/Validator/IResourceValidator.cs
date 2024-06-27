using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Resource;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;

public interface IResourceValidator
{
    /// <summary>
    /// Validates an access token.
    /// </summary>
    /// <param name="request">The access token.</param>
    /// <returns></returns>
    Task<ResourceValidationResult> ValidateResourceAsync(ResourceValidationRequest request);
}