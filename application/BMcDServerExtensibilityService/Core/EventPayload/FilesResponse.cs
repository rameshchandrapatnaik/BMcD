using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerExtensibility.Classes
{
    public partial class FilesResponse
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SPFFileComposition_21")]
        public FilesResponseValue[] Files { get; set; }
    }

    public partial class FilesResponseValue
    {
        [JsonProperty("UID")]
        public Guid Uid { get; set; }

        [JsonProperty("Class")]
        public string Class { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("OBID")]
        public string Obid { get; set; }

        [JsonProperty("Config")]
        public string Config { get; set; }

        [JsonProperty("LastUpdatedDate")]
        public DateTimeOffset LastUpdatedDate { get; set; }

        [JsonProperty("CreationDate")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("CreationUser")]
        public string CreationUser { get; set; }

        [JsonProperty("TerminationDate")]
        public DateTimeOffset TerminationDate { get; set; }

        [JsonProperty("TerminationUser")]
        public string TerminationUser { get; set; }

        [JsonProperty("DomainUID")]
        public string DomainUid { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("SPFNeedsIndexing")]
        public string SpfNeedsIndexing { get; set; }

        [JsonProperty("SPFIsFileCheckedOut")]
        public string SpfIsFileCheckedOut { get; set; }

        [JsonProperty("SPFViewInd")]
        public string SpfViewInd { get; set; }

        [JsonProperty("SPFRemoteFileName")]
        public string SpfRemoteFileName { get; set; }

        [JsonProperty("SPFLocalHostName")]
        public string SpfLocalHostName { get; set; }

        [JsonProperty("SPFLocalFileName")]
        public string SpfLocalFileName { get; set; }

        [JsonProperty("SPFFileFileType_12")]
        public FileResponseFileType[] FileTypes { get; set; }
    }

    public partial class FileResponseFileType
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }

}
