using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace BMcDExtensibilityService.Custom.Models
{
    public class HelperModels
    {
        public partial class ExecuteReportRequest
        {
            public bool CSVSupported { get; set; }
            public List<string> Parameters { get; set; }
            public List<string> SourceObjectIds { get; set; }
        }

        public partial class ExecuteReportResponse
        {
            public string FileSize { get; set; }
            public int ReportExecutionTimeMS { get; set; }
            public List<string> ResultColumns { get; set; }
            public int ResultCount { get; set; }
            public string ResultUrl { get; set; }
            public List<string> Results { get; set; }
        }

        public partial class AttachWorkflowRequest
        {
            public string ObjectOBID { get; set; }
            public string WorkflowNameOrUID { get; set; }
        }

        public partial class SimpleObject
        {
            public string UID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string OBID { get; set; }
            public string Config { get; set; }

            public static explicit operator SimpleObject(JsonObject jObj)
            {
                SimpleObject obj = new SimpleObject();

                object currPropValue;
                if (jObj.TryGetValue("UID", out currPropValue))
                {
                    obj.UID = (string)currPropValue;
                }
                if (jObj.TryGetValue("Name", out currPropValue))
                {
                    obj.Name = (string)currPropValue;
                }
                if (jObj.TryGetValue("Description", out currPropValue))
                {
                    obj.Description = (string)currPropValue;
                }
                if (jObj.TryGetValue("OBID", out currPropValue))
                {
                    obj.OBID = (string)currPropValue;
                }
                if (jObj.TryGetValue("Config", out currPropValue))
                {
                    obj.Config = (string)currPropValue;
                }
                return obj;
            }
        }

        public partial class ObjectResponse
        {
            [JsonProperty("@odata.count")]
            public string count { get; set; }
            public List<JsonObject> value { get; set; }
            [JsonProperty("@odata.nextLink")]
            public string nextItemsLink { get; set; }
        }

        public partial class SignOffPayload
        {
            public List<string> stepOBIDs { get; set; }
            public string signOffComment { get; set; }
            public string messageToNextStep { get; set; }
            public string acceptStep { get; set; }
        }
        public partial class RejectPayload
        {
            public List<string> stepOBIDs { get; set; }
            public string rejectComment { get; set; }
            public string messageToNextStep { get; set; }
            public string rejectStep { get; set; }
        }

        public partial class AttachWorkflowPayload
        {
            public string WorkflowObid { get; set;}
            public List<string> ObjectObids { get; set; }
        }

        public class FileUriResponse
        {
            [JsonProperty("@odata.context")]
            public string ODataContext { get; set; }

            [JsonProperty("value")]
            public List<RetrieveFileUris> Value { get; set; }
        }
        public partial class RetrieveFileUris
        {
            public string FileId { get; set; }
            public string Id { get; set; }
            public string ItemType { get; set; }
            public string Purpose { get; set; }
            public string Uri { get; set; }
            public string ContentLength { get; set; }
            public string ParentFileOBID { get; set; }


        }
    }
}
