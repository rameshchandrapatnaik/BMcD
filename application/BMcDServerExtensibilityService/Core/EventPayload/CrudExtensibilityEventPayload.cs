
namespace BMcDExtensibilityService.Core.EventPayload
{
    public class CrudExtensibilityEventPayload
    {
        public string OBID { get; set; }
        public string UID { get; set; }
        public string DomainUID { get; set; }
        public string[] InterfaceUIDs { get; set; }
        public string ClassDefUID { get; set; }
        public string ConfigUID { get; set; }
        public string User { get; set; }

    }
}