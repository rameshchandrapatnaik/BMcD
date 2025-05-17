using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace BMcDExtensibilityService.Core.Services
{
    /// <summary>
    /// Handles obtaining an OAuth token and refreshing expiring tokens
    /// </summary>
    public class AuthenticationService
    {
        public string token;
        public int mintTokenExpiryinSeconds;
        private readonly ExtensibilityConfiguration _config;
        public AuthenticationService(ExtensibilityConfiguration config)
        {
            _config = config;
        }
        public TokenResponse GetOAuthTokenClientCredentialsFlow(string authServerAuthority, string authClientId, string authClientSecret, string serverResourceID, string scope = "")
        {
            HttpClient client = new HttpClient();
            DiscoveryDocumentResponse discoveryDocument = client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authServerAuthority,
                Policy =
             {
             RequireHttps = false, //_config.GetValue<bool>("appSettings:IsRequiredHttps"), //false,
             ValidateEndpoints = false,
             ValidateIssuerName = false
             }
            }).Result;
            TokenResponse tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = authClientId,
                ClientSecret = authClientSecret,
                Scope = scope,
                //Parameters = new Dictionary<string, string>() { { "Resource", serverResourceID } }
                Parameters = new Parameters
    {
        { "resource", serverResourceID }
    }
            }).Result;
            if (!string.IsNullOrWhiteSpace(tokenResponse.Error))
            {
                Log.Information(tokenResponse.Error);
                throw new Exception(tokenResponse.Error);
            }
            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                Log.Information("Token obtained successfully");
                Console.WriteLine("Token obtained successfully");
            }
            token = tokenResponse.AccessToken;
            //Console.WriteLine("token = "+ token);
            //TokenExpirationCall(tokenResponse.ExpiresIn, authServerAuthority, authClientId, authClientSecret,serverResourceID, scope);
            mintTokenExpiryinSeconds = tokenResponse.ExpiresIn;
            return tokenResponse;
        }
        public TokenResponse GetOAuthTokenResourceOwnerFlow(string authServerAuthority, string authClientId, string authClientSecret,
        string serverResourceID, string authUsername,
        string authPassword, string scope = "")
        {
            HttpClient client = new HttpClient();
            DiscoveryDocumentResponse discoveryDocument = client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authServerAuthority,
                Policy =
                 {
                    RequireHttps = false,
                   ValidateEndpoints = false
                 }
            }).Result;
            TokenResponse tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = authClientId,
                ClientSecret = authClientSecret,
                UserName = authUsername,
                Password = authPassword,
                Scope = scope,
                Parameters = new Parameters
    {
        { "resource", serverResourceID }
    }
            }).Result;
            if (!string.IsNullOrWhiteSpace(tokenResponse.Error))
            {
                Log.Information(tokenResponse.Error);
            }
            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                Log.Information("Token obtained successfully");
                token = tokenResponse.AccessToken;
                TokenExpirationCallResourceOwner(tokenResponse.ExpiresIn, authServerAuthority, authClientId,
                authClientSecret, serverResourceID, authUsername, authPassword, scope);
            }
            return tokenResponse;
        }
        private void TokenExpirationCall(int timeTillExpires, string authServerAuthority, string authClientId,
        string authClientSecret, string serverResourceID, string scope = "")
        {
            // Sets token refresh callback before the token expires so that we dont get 401's by using expired tokens
            new System.Threading.Timer(e =>
            {
                try
                {
                    Log.Information("Token expiring obtaining new one...");
                    Console.WriteLine("Token expiring obtaining new one...");
                    GetOAuthTokenClientCredentialsFlow(authServerAuthority, authClientId, authClientSecret, serverResourceID, scope);
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception occured in token refresh timer callback: {ex}");
                }
            }
            , null, (timeTillExpires * 1000) - 30000, -1);
        }
        private void TokenExpirationCallResourceOwner(int timeTillExpires, string authServerAuthority, string authClientId,
        string authClientSecret, string serverResourceID,
        string authUsername, string authPassword, string scope = "")
        {
            // Sets token refresh callback before the token expires so that we dont get 401's by using expired tokens
            new System.Threading.Timer(e =>
            {
                Log.Information("Token expiring obtaining new one...");
                GetOAuthTokenResourceOwnerFlow(authServerAuthority, authClientId, authClientSecret, serverResourceID, authUsername, authPassword, scope);
            }
            , null, (timeTillExpires * 1000) - 30000, -1);
        }
        public void RenewToken(ExtensibilityConfiguration extensibilityConfig)
        {
            Log.Information("Token expiring obtaining new one...");
            if (extensibilityConfig.AuthUseClientCredentialsFlow)
            {
                Log.Information("Using client credentials flow...");
                //GetOAuthTokenClientCredentialsFlow(extensibilityConfig);
                GetOAuthTokenClientCredentialsFlow(extensibilityConfig.AuthServerAuthority, extensibilityConfig.AuthClientId, extensibilityConfig.AuthClientSecret,
                extensibilityConfig.ServerResourceID, "ingr.api");
            }
            else
            {
                Log.Information("Using resource owner flow ...");
                //GetOAuthTokenResourceOwnerFlow(extensibilityConfig);
                GetOAuthTokenResourceOwnerFlow(extensibilityConfig.AuthServerAuthority, extensibilityConfig.AuthClientId, extensibilityConfig.AuthClientSecret, extensibilityConfig.ServerResourceID,
                extensibilityConfig.AuthUsername, extensibilityConfig.AuthPassword, "ingr.api");
            }
        }
    }
}