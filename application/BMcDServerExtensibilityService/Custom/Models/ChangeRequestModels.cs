using Newtonsoft.Json;
using System.Collections.Generic;

namespace BMcDExtensibilityService.Custom.Models
{
    public class ChangeRequestModels
    {
        public partial class Classification
        {
            public string UID{ set; get; }
            public string Class{ set; get; }
            public string Name{ set; get; }
            public string OBID{ set; get; }

            [JsonProperty("CI_ClassificationShortID")] // TBD after schema creation
            public string ClassificationShortID{ set; get; }

            [JsonProperty("CI_IsAssetDependant")] // TBD after schema creation
            public string IsAssetDependant { set; get; }
        }

        public partial class Asset
        {
            public string UID { set; get; }
            public string Class { set; get; }
            public string Name { set; get; }
            public string OBID { set; get; }

            [JsonProperty("CI_PTTSimpleModWF")] // TBD or TBR after schema creation
            public string SimpleMODWF { set; get; }
        }

        public partial class ProjectForCreation
        {
            public string Class { set; get; }
            public string Name { set; get; }

            [JsonProperty("SPFConfigurationConfigurationStatus_12@odata.bind")]
            public List<string> ConfigStatus { set; get; }

            [JsonProperty("SPFConfigurationTree_21@odata.bind")]
            public List<string> Parent { set; get; }

            [JsonProperty("PTTEPChangeRequestProject_21@odata.bind")]
            public List<string> Modification { set; get; }
        }
    }
}
