using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerExtensibility.Classes
{
    public partial class RetrieveFilePayload
    {
        [JsonProperty("purposes")]
        public string[] Purposes { get; set; }

        [JsonProperty("downloadFile")]
        public bool DownloadFile { get; set; }
    }
}
