using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerExtensibility.Classes
{
    public partial class RetrieveFileResponse
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("value")]
        public RetrievedFile[] Value { get; set; }
    }

    public partial class RetrievedFile
    {
        [JsonProperty("FileId")]
        public string FileId { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ItemType")]
        public string ItemType { get; set; }

        [JsonProperty("Purpose")]
        public string Purpose { get; set; }

        [JsonProperty("Uri")]
        public string Uri { get; set; }

        [JsonProperty("ContentLength")]
        
        public string ContentLength { get; set; }

        [JsonProperty("ParentFileOBID")]
        public string ParentFileObid { get; set; }
    }
}
