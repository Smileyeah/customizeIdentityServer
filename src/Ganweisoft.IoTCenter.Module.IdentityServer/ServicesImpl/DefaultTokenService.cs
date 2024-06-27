// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl
{
    /// <summary>
    /// Default token service
    /// </summary>
    public class DefaultTokenService : ITokenService
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
        /// Initializes a new instance of the <see cref="DefaultTokenService" /> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="logger">The logger.</param>
        public DefaultTokenService(
            ISystemClock clock,
            ILogger<DefaultTokenService> logger)
        {
            Clock = clock;
            Logger = logger;
        }
        
        /// <summary>
        /// Creates an access token.
        /// </summary>
        /// <param name="request">The token creation request.</param>
        /// <returns>
        /// An access token
        /// </returns>
        public virtual async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            Logger.LogDebug("Creating access token");
            var token = new Token
            {
                CreationTime = Clock.UtcNow.UtcDateTime,
                Issuer = "ganweicloud",
                Lifetime = request.AccessTokenLifetime,
                Claims = request.Subject.Claims.ToHashSet(),
                AppId = request.AppId,
                Description = request.Description,
                AllowedSigningAlgorithms = new [] { "SHA256" }
            };
            
            // source user add system claim
            token.Claims.Add(new Claim(IdentityServerConstant.GwClientClaim, request.AppId));
            if (string.IsNullOrWhiteSpace(request.Nonce))
            {
                token.Claims.Add(new Claim(ClaimTypes.PostalCode, request.Nonce));
            }

            return await Task.FromResult(token);
        }
    }
}