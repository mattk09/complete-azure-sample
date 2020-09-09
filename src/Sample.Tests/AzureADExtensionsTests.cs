using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Sample.Extensions;
using Sample.Extensions.Configurations;
using Xunit;

namespace Sample.Tests
{
    public class AzureADExtensionsTests
    {
        [Fact]
        public void AuthenticationServiceAvailableWhenAuthConfigEnabled()
        {
            var authConfig = new AuthenticationConfiguration();
            authConfig.Enabled = true;
            authConfig.ActiveDirectory = new AzureADConfiguration();

            var provider = BuildServiceProvider(services =>
            {
                services.AddAzureAdAuthentication(authConfig);
            });

            var exception = Record.Exception(() => provider.GetRequiredService<IAuthenticationService>());
            Assert.Null(exception);
        }

        [Fact]
        public void AuthenticationServiceNotAvailableWhenAuthConfigIsDisabled()
        {
            var authConfig = new AuthenticationConfiguration();
            authConfig.Enabled = false;
            authConfig.ActiveDirectory = new AzureADConfiguration();

            var provider = BuildServiceProvider(services =>
            {
                services.AddAzureAdAuthentication(authConfig);
            });

            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IAuthenticationService>());
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenActiveDirectoryConfigurationIsNull()
        {
            var authConfig = new AuthenticationConfiguration();
            authConfig.Enabled = true;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = BuildServiceProvider(services =>
                {
                    services.AddAzureAdAuthentication(authConfig);
                });
            });
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenAuthConfigurationIsNull()
        {
            AuthenticationConfiguration authConfig = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = BuildServiceProvider(services =>
                {
                    services.AddAzureAdAuthentication(authConfig);
                });
            });
        }

        private IServiceProvider BuildServiceProvider(
            Action<IServiceCollection> setupServices = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            setupServices?.Invoke(services);
            return services.BuildServiceProvider();
        }
    }
}
