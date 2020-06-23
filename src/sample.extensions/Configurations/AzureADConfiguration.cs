using System;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;

namespace Sample.Extensions.Configurations
{
    public class AzureADConfiguration
    {
        private const string InvalidRangeMessage = "AccessTokenVersion valid values {1 | 2}";
        private int accessTokenVersion;

        public string Domain { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string Authority { get; set; }

        public Uri ReplyUri { get; set; }

        public int AccessTokenVersion
        {
            get => accessTokenVersion;

            set
            {
                if (value <= 0 && value > 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(AccessTokenVersion), value, InvalidRangeMessage);
                }

                accessTokenVersion = value;
            }
        }

        public AzureADConfiguration()
        {
        }
    }
}
