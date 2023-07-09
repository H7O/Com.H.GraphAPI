using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace Com.H.GraphAPI.Identity
{
    public static class GIExtensions
    {
        public static async Task<AuthenticationResult> RequestAccessTokenAsync(
            string clientId,
            string clientSecret,
            string tenantId)
        {

            // Set up the authentication context and acquire a token

            var authBuilder = ConfidentialClientApplicationBuilder.Create(clientId)

                .WithAuthority($"https://login.microsoftonline.com/{tenantId}/v2.0")
                .WithClientSecret(clientSecret)
            .Build();

            return await authBuilder.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync();
        }

        public static AuthenticationResult RequestAccessToken(
            string clientId,
            string clientSecret,
            string tenantId)
        {
            return RequestAccessTokenAsync(clientId, clientSecret, tenantId).GetAwaiter().GetResult();
        }

        public static async Task<string> GetAccessTokenAsync(
            string clientId,
            string clientSecret,
            string tenantId)
        {
            var authResult = await RequestAccessTokenAsync(clientId, clientSecret, tenantId);
            return authResult.AccessToken;
        }

        public static string GetAccessToken(
            string clientId,
            string clientSecret,
            string tenantId)
        {
            return GetAccessTokenAsync(clientId, clientSecret, tenantId).GetAwaiter().GetResult();
        }

        public static async Task<(string AccessToken, DateTimeOffset ExpiresOn)> GetAccessTokenWithExpiryDateAsync(
            string clientId,
            string clientSecret,
            string tenantId)
        {
            var authResult = await RequestAccessTokenAsync(clientId, clientSecret, tenantId);
            return (authResult.AccessToken, authResult.ExpiresOn);
        }

        public static (string AccessToken, DateTimeOffset ExpiresOn) GetAccessTokenWithExpiryDate(
            string clientId,
            string clientSecret,
            string tenantId)
        {
            return GetAccessTokenWithExpiryDateAsync(clientId, clientSecret, tenantId).GetAwaiter().GetResult();
        }
    }
}
