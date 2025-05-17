using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Models
{
    public class MarkupLayerModel
    {
        public class MarkupFileObj
        {
            public string Class { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string SPFLocalFileName { get; set; }
            public string SPFIsMarkupConsolidated { get; set; }

            [JsonProperty("SPFFileMarkup_21@odata.bind")]
            public string SPFFileMarkup_21 { get; set; }
			[JsonProperty("SPFItemOwningGroup_12@odata.bind")]
			public string SPFItemOwningGroup_12 { get; set; }
			public string SPFMarkupColor { get; internal set; }
		}
        
        public class CreateMarkupObj
        {
            public string UID { get; set; }
            public string Class { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string OBID { get; set; }
            public string SPFRemoteFileName { get; set; }
            public string SPFLocalFileName { get; set; }
            public string SPFMarkupType { get; set; }
            public string Config { get; set; }
            public string CreationDate { get; set; }
            public string CreationUser { get; set; }


        }

        public partial class CreateMarkUp
        {
            public string MarkupOBID { get; set; }
            public string MarkupContent { get; set; }
            public string MarkupContextObjectOBID { get; set; }
            public string RelDefFromContextObjectToMarkupFile { get; set; }
            public string RelDefForRenditionGeneration { get; set; }
            public string ObjectOBIDForRenditionGeneration { get; set; }
	
                  public bool IsBatchRequest { get; set; }

		}

        public class CreateMarkupObjRes
        {
            public string UID { get; set; }
            public string Class { get; set; }
            public string Name { get; set; }
            public string CI_Description { get; set; }
            public string OBID { get; set; }
            public string CI_OwningGroup { get; set; }
            public string CI_CreationUser { get; set; }

        }
        public partial class SaveMarkup
        {
            public string ActionInterface { get; set; }
            public string ActionPinOBIDs { get; set; }
            public string MarkupContent { get; set; }
            public string MarkupOBID { get; set; }
            public string RelDefForActionRenditionGen { get; set; }
        }

        public partial class SaveMarkupRes
        {
            public string value { get; set; }
        }

        public class ItemOwner
        {
            public string UID { get; set; }
            public string Class { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string OBID { get; set; }
            public string SPFRemoteFileName { get; set; }
            public string SPFLocalFileName { get; set; }
            public string SPFMarkupType { get; set; }
            public string Config { get; set; }
            public SPFItemOwner12 SPFItemOwner_12 { get; set; }
        }
        public class SPFItemOwner12
        {
            public string UID { get; set; }
            public string Class { get; set; }
            public string Name { get; set; }
            public string SPFEmailAddress { get; set; }
            public string OBID { get; set; }
        }
    }
}
