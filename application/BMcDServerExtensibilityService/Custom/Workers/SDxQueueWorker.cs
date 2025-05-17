using Microsoft.Extensions.Hosting;
using BMcDExtensibilityService.Core;
using BMcDExtensibilityService.Core.Handlers;
using BMcDExtensibilityService.Core.Handlers.Models;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Workers
{
    public class SDxQueueWorker : BackgroundService
    {

        private ExtensibilityODataClient _extensibilityODataClient;
        private ExtensibilityConfiguration _extensibilityConfiguration;
        private ExtensibilityEventHandler _extensibilityEventHandler;
        private AMQPHandler _amqpHandler;
        private AzureAMQPHandler _azureAMQPHandler;

        private IHandler amqpHandler;

        public SDxQueueWorker(ExtensibilityConfiguration config,
            ExtensibilityODataClient extensibilityODataClient,
            ExtensibilityEventHandler extensibilityEventHandler,
            AMQPHandler amqpHandler,
            AzureAMQPHandler azureAMQPHandler
            )
        {
           _extensibilityConfiguration = config;
            _extensibilityODataClient = extensibilityODataClient;
            _extensibilityEventHandler = extensibilityEventHandler;
            _amqpHandler = amqpHandler;
            _azureAMQPHandler = azureAMQPHandler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Log.Information("Worker running at: {time}", DateTimeOffset.Now);

            Task.Run(() => ProcessMessagesFromSdx(stoppingToken));
            return Task.CompletedTask;

        }


        private void ProcessMessagesFromSdx(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!string.IsNullOrWhiteSpace(_extensibilityConfiguration.AzureAuthServerAuthority))
                {
                    /// Setup Azure AMQP Listener
                    amqpHandler = _azureAMQPHandler;
                }
                else
                {
                    /// Setup AMQP Listener
                    amqpHandler = _amqpHandler;
                }
                amqpHandler.SetupAMQPAndCallback(_extensibilityConfiguration, _extensibilityODataClient, _extensibilityEventHandler);
            }
        }
    }
}

