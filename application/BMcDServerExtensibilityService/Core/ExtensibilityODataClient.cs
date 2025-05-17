using BMcDExtensibilityService.Core.Services;
using BMcDExtensibilityService.Core;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
namespace BMcDExtensibilityService.Core
{
    /// <summary>
    /// Responsible for setting up the ODataClient correctly to commincate with SDX/SPF
    /// </summary>
    public class ExtensibilityODataClient : ODataClient
    {
        public ExtensibilityConfiguration extensibilityConfiguration;
        public Dictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        public AuthenticationService _authenticationService;
        public List<string> correlationIds = new List<string>();
        private const string correlationIdKey = "X-Correlation-ID";
        public ExtensibilityODataClient(ExtensibilityConfiguration config, AuthenticationService authenticationService,
        List<string> correlationIds) : base(
        GetClientSettings(config, correlationIds))
        {
            this.correlationIds = correlationIds;
            this.extensibilityConfiguration = config;
            //authenticationService = new AuthenticationService();
            _authenticationService = authenticationService;
            if (config.AuthUseClientCredentialsFlow)
            {
                _authenticationService.GetOAuthTokenClientCredentialsFlow(config.AuthServerAuthority, config.AuthClientId, config.AuthClientSecret, config.ServerResourceID, "ingr.api");
            }
            else
            {
                _authenticationService.GetOAuthTokenResourceOwnerFlow(config.AuthServerAuthority, config.AuthClientId, config.AuthClientSecret, config.ServerResourceID,
                config.AuthUsername, config.AuthPassword, "ingr.api");
            }
            /// Initial configuration
            headers.Add("Accept", new List<string>() { "application/vnd.intergraph.data+json" });
            headers.Add("Authorization", new List<string>() { "Bearer " + authenticationService.token });
            headers.Add("SPFResolveDocumentsToVersion", new List<string>() { "true" });
            UpdateRequestHeaders(headers);
        }
        /// <summary>
        /// Sets the SPFConfigUID header to the configuration provided
        /// </summary>
        /// <param name="configUID"></param>
        public void SetConfigHeader(string configUID)
        {
            if (headers.ContainsKey("spfconfiguid"))
            {
                headers["spfconfiguid"] = new List<string>() { configUID };
            }
            else
            {
                headers.Add("spfconfiguid", new List<string>() { configUID });
            }
            UpdateRequestHeaders(headers);
        }
        /// <summary>
        /// Sets the X-Ingr-OnBehalfOf header to the value of the user provided
        /// which will be used server side for impersonation
        /// </summary>
        /// <param name="username"></param>
        public void SetImpersonationHeader(string username)
        {
            if (extensibilityConfiguration.AuthUseClientCredentialsFlow)
            {
                if (headers.ContainsKey("X-Ingr-OnBehalfOf"))
                {
                    headers["X-Ingr-OnBehalfOf"] = new List<string>() { username };
                }
                else
                {
                    headers.Add("X-Ingr-OnBehalfOf", new List<string>() { username });
                }
                UpdateRequestHeaders(headers);
            }
        }
        private static ODataClientSettings GetClientSettings(ExtensibilityConfiguration config, List<string> correlationIds)
        {
            var serverUri = config.ServerBaseUri + "/SDA";
            ODataClientSettings lobjClientSettings = new ODataClientSettings(new Uri(serverUri));
            lobjClientSettings.AfterResponse = (HttpResponseMessage message) =>
            {
                if (message.Headers.Contains(correlationIdKey))
                {
                    string correlationId = message.Headers.GetValues(correlationIdKey).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(correlationId))
                    {
                        correlationIds.Add(correlationId);
                    }
                }
            };
            return lobjClientSettings;
        }
        //added
        public void UpdateODataToken(string pstrRenewedToken)
        {
            headers["Authorization"] = new List<string>() { "Bearer" + pstrRenewedToken };
        }
    }
}