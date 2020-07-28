using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sample.Exceptions;
using Sample.Extensions.Interfaces;

namespace Sample.Extensions
{
    public static class AzureADExtension
    {
        public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IAuthenticationConfiguration authConfig)
        {
            Guard.ThrowIfNull(authConfig, nameof(authConfig));

            // Add even if disabled so dependencies can activate
            services.AddSingleton<IAuthenticationConfiguration>(authConfig);

            if (!authConfig.Enabled)
            {
                return services;
            }

            Guard.ThrowIfNull(authConfig.ActiveDirectory, nameof(authConfig.ActiveDirectory));

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
