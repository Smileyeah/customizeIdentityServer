using System;
using System.Linq;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Resource;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Validator;

public class DefaultResourceValidator : IResourceValidator
{
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger Logger;
    
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly IConfiguration Configuration;
    
    /// <summary>
    /// 
    /// </summary>
    protected readonly IdentityServerDbContext ApplicationDbContext;

    public DefaultResourceValidator(ILogger<DefaultTokenService> logger,
        IdentityServerDbContext dbContext, IConfiguration configuration)
    {
        Logger = logger;
        ApplicationDbContext = dbContext;
        Configuration = configuration;
    }
    
    /// <summary>
    /// Validate resource belong to user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ResourceValidationResult> ValidateResourceAsync(ResourceValidationRequest request)
    {
        var enableExpectedScope = Configuration.GetSection("IdentityServer:EnableExpectedScope").Get<bool>();
        if (enableExpectedScope)
        {
            var expectedScopeStr = Configuration.GetSection("IdentityServer:ExpectedScope").Get<string[]>();
            var scope = expectedScopeStr.Any(e => e.Equals(request.Scope, StringComparison.OrdinalIgnoreCase));
            if (scope)
            {
                Logger.LogError($"Resource validate for expected scope: {request.Scope} success");
                
                return new ResourceValidationResult
                {
                    IsValid = true
                };
            }
        }

        return await Task.FromResult(new ResourceValidationResult
        {
            IsValid = true
        });
    }
}