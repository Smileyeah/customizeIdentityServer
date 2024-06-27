using System;
using System.Linq;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Generator;

/// <summary>
/// The authorize response generator
/// </summary>
public class AuthorizeResponseGenerator : IAuthorizeResponseGenerator
{
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// The clock
    /// </summary>
    protected readonly ISystemClock Clock;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly IAuthorizationCodeStore Store;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeResponseGenerator"/> class.
    /// </summary>
    /// <param name="clock">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="store">The authorization code store.</param>
    public AuthorizeResponseGenerator(
        ISystemClock clock,
        ILogger<AuthorizeResponseGenerator> logger,
        IAuthorizationCodeStore store)
    {
        Clock = clock;
        Logger = logger;
        Store = store;
    }

    /// <summary>
    /// Creates the response
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">invalid grant type: " + request.GrantType</exception>
    public virtual async Task<AuthorizeResponse> CreateResponseAsync(ValidatedAuthorizeRequest request)
    {
        return await CreateCodeFlowResponseAsync(request);
    }

    /// <summary>
    /// Creates the response for a code flow request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual async Task<AuthorizeResponse> CreateCodeFlowResponseAsync(ValidatedAuthorizeRequest request)
    {
        Logger.LogDebug("Creating Authorization Code Flow response.");

        var code = await CreateCodeAsync(request);

        var id = await Store.StoreAuthorizationCodeAsync(code);
            
        request.RequestedScopes = request.Scope.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

        var response = new AuthorizeResponse
        {
            Request = request,
            Code = id
        };

        return response;
    }


    /// <summary>
    /// Creates an authorization code
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual async Task<AuthorizationCode> CreateCodeAsync(ValidatedAuthorizeRequest request)
    {
        var code = new AuthorizationCode
        {
            CreationTime = Clock.UtcNow.UtcDateTime,
            AppId = request.AppId,
            Lifetime = request.AuthorizationCodeLifetime,
            Subject = request.Subject,
            Description = request.Description,
            RedirectUri = request.RedirectUri,
            Nonce = request.Nonce,
            StateHash = request.State,
            RequestedScopes = request.RequestedScopes
        };

        return await Task.FromResult(code);
    }
}