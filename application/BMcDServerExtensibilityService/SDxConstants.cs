using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService
{
    public static class SDxConstants
    {
        public const string ChangeRequestClass = "SDAChangeRequest";
        public const string ChangeRequestPrimaryClassificationRel = "SPFPrimaryClassification_21";
        public const string ChangeRequestAssetRel = "SDAChangeRequestAreas_12";
        public const string ChangeRequestIdProperty = "PTTEPModificationNumber";

        public const string ProjectStatusClass = "SPFConfigurationStatus";
        public const string ProjectStatusDefaultValue = "Active";
        public const string ProjectPlantClass = "SPFPlant";
        public const string ProjectPlantName = "PlantA";
        public const string ProjectClass = "SPFProject";
    }
}
