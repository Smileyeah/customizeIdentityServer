using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;

/// <summary>
/// Interface for the authorize response generator
/// </summary>
public interface IAuthorizeResponseGenerator
{
    /// <summary>
    /// Creates the response
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    Task<AuthorizeResponse> CreateResponseAsync(ValidatedAuthorizeRequest request);
}