using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Core.EventPayload
{
    internal class SimpleObjectPayload
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string CI_Name { get; set; }
        public string CI_Description { get; set; }
        public string CI_Class { get; set; }
        public string CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public string CI_LastUpdateDate { get; set; }
        public string Config { get; set; }
        public string LastUpdatedDate { get; set; }
    }

    public class TestModel
    {
        public List<Dictionary<string, string>> Item { get; set; }
    }
}
