
using System;
using System.Collections.Generic;

namespace BMcDExtensibilityService.Core.EventPayload
{
    public class UpdateExtensibilityEventPayload: CrudExtensibilityEventPayload
    {
        public Dictionary<String, List<String>> UpdatedProperties { get; set; }

    }
}