using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Configuration;
using Sample.Extensions.Configurations;
using Sample.Extensions.Interfaces;
using Xunit;

namespace Sample.Tests
{
    public class AuthenticationConfigurationTests
    {
        private const string AppSettings = "{\"Authentication\":{\"Enabled\":true,\"ActiveDirectory\":{\"Domain\":\"xxxx.onmicrosoft.com\",\"TenantId\":\"tenantid\",\"ClientId\":\"clientid\", \"ReplyUri\":\"https://localhost/login\",\"Authority\": \"https://login.microsoftonline.com/{TenantId}/V2.0\"}}}";
        private const string AuthenticationConfigSection = "Authentication";

        [Fact]
        public void CreateAuthConfigUsingValidConfiguration()
        {
            var authConfig = GetAuthConfig(AppSettings, AuthenticationConfigSection);

            Assert.NotNull(authConfig);
            Assert.True(authConfig.Enabled);
            Assert.NotNull(authConfig.ActiveDirectory);
        }

        [Fact]
        public void PopulatedAzureAdOptionsWithJsonValues()
        {
            var authConfig = GetAuthConfig(AppSettings, AuthenticationConfigSection);

            var azureAdOptions = new AzureADOptions();
            authConfig.UpdateAzureAdOptions(azureAdOptions);

            Assert.Equal("https://login.microsoftonline.com/", azureAdOptions.Instance);
            Assert.Equal("xxxx.onmicrosoft.com", azureAdOptions.Domain);
            Assert.Equal("tenantid", azureAdOptions.TenantId);
            Assert.Equal("clientid", azureAdOptions.ClientId);
        }

        private IAuthenticationConfiguration GetAuthConfig(string jsonString, string configSection)
        {
            return GetConfiguration(jsonString).GetSection(configSection)
                .Get<AuthenticationConfiguration>();
        }

        private IConfigurationRoot GetConfiguration(string jsonString)
        {
            using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonStream(jsonStream);

                return builder.Build();
            }
        }
    }
}
