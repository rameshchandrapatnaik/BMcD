using System;
using System.Collections.Generic;
using System.Text;

namespace BMcDExtensibilityService.Core.EventPayload
{
    class RelExtensibilityEventPayload
    {
        public string OBID { get; set; }
        public string DomainUID { get; set; }
        public string End1UID { get; set; }
        public string End1Domain { get; set; }
        public string End1OBID { get; set; }
        public string End2UID { get; set; }
        public string End2Domain { get; set; }
        public string End2OBID { get; set; }
        public string[] InterfaceUIDs { get; set; }
        public string RelDefUID { get; set; }
        public string ConfigUID { get; set; }
        public string User { get; set; }
    }
}
