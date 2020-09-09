using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Primitives;
using Sample.Extensions.Configurations;

namespace Sample.Extensions.Interfaces
{
    public interface IAuthenticationConfiguration
    {
        public bool Enabled { get; }        

        public AzureADConfiguration ActiveDirectory { get; }

        public void UpdateAzureAdOptions(AzureADOptions options);
        
        Uri GetAuthorizationUri(string state, string nonce);
    }
}
