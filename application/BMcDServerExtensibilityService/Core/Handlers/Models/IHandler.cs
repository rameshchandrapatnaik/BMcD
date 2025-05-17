namespace BMcDExtensibilityService.Core.Handlers.Models
{
    public interface IHandler
    {
        void SetupAMQPAndCallback(ExtensibilityConfiguration extensibilityConfiguration, ExtensibilityODataClient extensibilityODataClient, ExtensibilityEventHandler extensibilityEventHandler);
    }
}