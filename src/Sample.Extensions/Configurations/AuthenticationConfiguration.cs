using System;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Sample.Exceptions;
using Sample.Extensions.Interfaces;

namespace Sample.Extensions.Configurations
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        private const string AzureAdEndpoint = "https://login.microsoftonline.com/";

        public bool Enabled { get; set; }

        public AzureADConfiguration ActiveDirectory { get; set; }

        private string Version => ActiveDirectory.AccessTokenVersion == 2 ? "v2.0/" : string.Empty;

        public AuthenticationConfiguration()
        {
        }

        public void UpdateAzureAdOptions(AzureADOptions options)
        {
            Guard.ThrowIfNull(options, nameof(options));

            options.Instance = AzureAdEndpoint;
            options.Domain = ActiveDirectory.Domain;
            options.TenantId = ActiveDirectory.TenantId;
            options.ClientId = ActiveDirectory.ClientId;
        }

        public Uri GetAuthorizationUri(string state, string nonce)
        {
            // In this sample we are using OpenId Connect https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc
            // State and Nonce can be used to validate the login response and the id_token, respectively.
            // To understand the behavior of each parameters check. https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc#send-the-sign-in-request
            return new Uri($"{AzureAdEndpoint}" +
                $"{ActiveDirectory.TenantId}/oauth2/{Version}authorize?" +
                $"client_id={ActiveDirectory.ClientId}&" +
                $"response_type=id_token&" +
                $"redirect_uri={ActiveDirectory.ReplyUri}&" +
                $"response_mode=form_post&" +
                $"scope=openid%20offline_access&" +
                $"state={state}&" +
                $"nonce={nonce}");
        }
    }
}
