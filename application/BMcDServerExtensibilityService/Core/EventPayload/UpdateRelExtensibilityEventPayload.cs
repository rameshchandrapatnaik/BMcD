using System;
using System.Collections.Generic;
using System.Text;

namespace BMcDExtensibilityService.Core.EventPayload
{
    class UpdateRelExtensibilityEventPayload : RelExtensibilityEventPayload
    {
        public Dictionary<String, List<String>> UpdatedProperties { get; set; }
    }
}
