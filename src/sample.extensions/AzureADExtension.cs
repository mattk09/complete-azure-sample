using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sample.Extensions.Interfaces;

namespace Sample.Extensions
{
    public static class AzureADExtension
    {
        public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IAuthenticationConfiguration authConfig)
        {
            if (authConfig == null)
            {
                throw new ArgumentNullException(nameof(authConfig));
            }

            if (!authConfig.Enabled)
            {
                return services;
            }

            if (authConfig.ActiveDirectory == null)
            {
                throw new ArgumentNullException($"{nameof(authConfig)}.ActiveDirectory");
            }

            services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
               .AddAzureADBearer(options => authConfig.UpdateAzureAdOptions(options));

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {
                options.Authority = authConfig.ActiveDirectory.Authority;
                options.Audience = authConfig.ActiveDirectory.ClientId;
            });

            return services;
        }
    }
}
