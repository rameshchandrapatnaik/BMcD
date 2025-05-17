using IdentityModel.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using BMcDExtensibilityService.Core.Handlers.Models;
using BMcDExtensibilityService.Core.Services;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Core.Handlers
{
    /// <summary>
    /// Class to get OAuth token from AzureAD for Azure Service bus
    /// </summary>
    public class AzureAMQPHandler : IHandler
    {

        private AuthenticationService _authenticationService;
        private SubscriptionClient receiveClient;

        public AzureAMQPHandler(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public void SetupAMQPAndCallback(ExtensibilityConfiguration config, ExtensibilityODataClient extensibilityODataClient,
            ExtensibilityEventHandler extensibilityEventHandler)
        {
            ITokenProvider tokenProvider = TokenProvider.CreateAzureActiveDirectoryTokenProvider(async (audience, authority, state) =>
                    {
                        TokenResponse response = _authenticationService.GetOAuthTokenClientCredentialsFlow($"https://login.microsoftonline.com/{config.AzureAuthServerAuthority}/",
                            config.AzureAuthClientId,config.AzureAuthClientSecret, config.AzureResourceID);
                        return response.AccessToken;
                    }, $"https://login.windows.net/{config.AzureAuthServerAuthority}/");

            string[] subSplit = config.AMQPBrokerQueue.Split("/Subscriptions/");

            receiveClient = new SubscriptionClient(new Uri($"sb://{config.AMQPBrokerHostname}/").ToString(), subSplit[0], subSplit[1], tokenProvider);
            Log.Information("Waiting for messages");

            this.receiveClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    extensibilityEventHandler.ProcessEventMessage(extensibilityODataClient, message.Label, message.Body);

                    await this.receiveClient.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions((eventargs) => { return Task.CompletedTask; })
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                });
        }

    }
}
