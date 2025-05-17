using Amqp;
using BMcDExtensibilityService.Core.Handlers.Models;
using Serilog;
using System;

namespace BMcDExtensibilityService.Core.Handlers
{
    public class AMQPHandler : IHandler
    {

        private Session session;
        private ReceiverLink receiver;
        private Connection connection;

        public void SetupAMQPAndCallback(ExtensibilityConfiguration extensibilityConfiguration, ExtensibilityODataClient extensibilityODataClient, 
            ExtensibilityEventHandler extensibilityEventHandler)
        {
            Address address = new Address(extensibilityConfiguration.AMQPBrokerHostname, extensibilityConfiguration.AMQPBrokerPort, extensibilityConfiguration.AMQPBrokerUsername, extensibilityConfiguration.AMQPBrokerPassword, "/", extensibilityConfiguration.AMQPBrokerProtocol);

            // Create necessary objects to receive events from broker and recreate if objects are closed
            if (connection == null || connection.IsClosed)
            {
                connection = new Connection(address);

                connection.AddClosedCallback(new ClosedCallback((o, e) =>
                {
                    Log.Information("Connection has closed, recreating connection");

                    SetupAMQPAndCallback(extensibilityConfiguration, extensibilityODataClient, extensibilityEventHandler);
                }));
            }

            if (session == null || session.IsClosed)
            {
                session = new Session(connection);
                session.AddClosedCallback(new ClosedCallback((o, e) =>
                {
                    Log.Information("Session has closed, recreating session");
                    SetupAMQPAndCallback(extensibilityConfiguration, extensibilityODataClient, extensibilityEventHandler);
                }));
            }

            if (receiver == null || receiver.IsClosed)
            {
                receiver = new ReceiverLink(session, "receiver-link", extensibilityConfiguration.AMQPBrokerQueue);
                receiver.AddClosedCallback(new ClosedCallback((o, e) =>
                {
                    Log.Information("ReceiverLink has closed, recreating ReceiverLink");
                    SetupAMQPAndCallback(extensibilityConfiguration, extensibilityODataClient, extensibilityEventHandler);
                }));
            }

           // Console.WriteLine("Waiting for messages");

            // Setup the callback which is called when an event is received
            MessageCallback callback = new MessageCallback((link, message) =>
            {
                receiver.Accept(message); 
                Log.Information(message.Properties.Subject);

                try
                {
                    if (!extensibilityODataClient.correlationIds.Contains(message.Properties.CorrelationId))
                    {
						//Log.Information("Phani processing the event message : " + message.Properties.Subject);
						extensibilityEventHandler.ProcessEventMessage(extensibilityODataClient, message.Properties.Subject, message.Body);
                    }
                    else
                    {                        
                        //Log.Information("Phani Event received was triggered by custom code so ignoring event");
                        extensibilityODataClient.correlationIds.Remove(message.Properties.CorrelationId);
                    }

                }
                catch (Exception e)
                {
                    Console.Write(e.InnerException.ToString());
                }
				//receiver.Accept(message);
			});
            // Set link credit to stop receiver being overloaded with events see: https://azure.github.io/amqpnetlite/articles/building_application.html#link-credit
            receiver.Start(100, callback);

        }

    }
}
