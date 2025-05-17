using IdentityModel.Client;
using BMcDExtensibilityService.Core.Services;
using RestSharp;
using System.Net.Http;
namespace BMcDExtensibilityService.Core
{
    public class ExtensibilityRestClient
    {
        private ExtensibilityConfiguration _config;
        public RestClient restClient;
        public RestClient restClientBase;
        public HttpClient httpClient;
        private AuthenticationService authService;
        public string strToken
        {
            get { return authService.token; }
        }
        public ExtensibilityRestClient(ExtensibilityConfiguration configuration, AuthenticationService authenticationService)
        {
            _config = configuration;
            authService = authenticationService;
            string serverUri = _config.ServerBaseUri;
            restClient = new RestClient(serverUri);
            restClient.RemoveDefaultParameter("Accept");
            restClient.AddDefaultHeader("Authorization", "Bearer " + authenticationService.token);
            restClientBase = new RestClient(_config.ServerBaseUri);
            restClientBase.RemoveDefaultParameter("Accept");
            restClientBase.AddDefaultHeader("Authorization", "Bearer " + authenticationService.token);
            httpClient = new HttpClient();
            //httpClient.BaseAddress = new System.Uri(_config.ServerBaseUri);
            httpClient.SetBearerToken(authenticationService.token);
        }
        public void SetImpersonationHeader(string username)
        {
            if (_config.AuthUseClientCredentialsFlow)
            {
                restClient.AddDefaultHeader("X-Ingr-OnBehalfOf", username);
            }
        }
        public void UpdateTokenForRESTandHTTPClient(string pstrToken)
        {
            restClient.RemoveDefaultParameter("Authorization");
            restClient.AddDefaultHeader("Authorization", "Bearer" + pstrToken);
            restClientBase.RemoveDefaultParameter("Authorization");
            restClientBase.AddDefaultHeader("Authorization", "Bearer" + pstrToken);
            httpClient.SetBearerToken(pstrToken);
        }
    }
}