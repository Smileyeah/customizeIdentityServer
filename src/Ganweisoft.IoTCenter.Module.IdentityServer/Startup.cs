using System;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Store;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Validator;
using Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl;
using Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Generator;
using Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Store;
using Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Validator;
using IoTCenter.Data.Internal.Extensions;
using IoTCenterCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ganweisoft.IoTCenter.Module.IdentityServer
{
    public class Startup : StartupBase
    {
        public IIoTConfiguration Configuration { get; set; }

        public Startup(IIoTConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddIoTDbContext<IdentityServerDbContext>(Configuration);
            
            services.TryAddScoped<ITokenResponseGenerator, TokenResponseGenerator>();
            services.TryAddScoped<IAuthorizeResponseGenerator, AuthorizeResponseGenerator>();
            services.TryAddScoped<IUserInfoResponseGenerator, UserInfoResponseGenerator>();
            services.TryAddScoped<IAuthorizeInteractionResponseGenerator, AuthorizeInteractionResponseGenerator>();
        
            services.TryAddScoped<ITokenService, DefaultTokenService>();
            services.TryAddScoped<ITokenValidator, DefaultTokenValidator>();
            services.TryAddScoped<IResourceValidator, DefaultResourceValidator>();
        
            services.TryAddScoped<IAuthorizationCodeStore, DefaultAuthorizationCodeStore>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            routes.MapDefaultControllerRoute();
        }
    }
}