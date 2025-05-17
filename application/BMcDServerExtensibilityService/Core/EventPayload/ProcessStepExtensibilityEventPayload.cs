
namespace BMcDExtensibilityService.Core.EventPayload
{
    public class ProcessStepExtensibilityEventPayload
    {
        public string OBID { get; set; }
        public string UID { get; set; }
        public string DomainUID { get; set; }
        public string WorkflowStepClassName { get; set; }
        public string WorkflowObjectObid { get; set; }
        public string ConfigUID { get; set; }
        public string User { get; set; }

    }
}