using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Services;

/// <summary>
/// Logic for creating security tokens
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates an access token.
    /// </summary>
    /// <param name="request">The token creation request.</param>
    /// <returns>An access token</returns>
    Task<Token> CreateAccessTokenAsync(TokenCreationRequest request);
}