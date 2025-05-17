using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Models
{
    public class SCLBSubmittals
    {
        public string odatacontext { get; set; }
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string SPFXmtlIssueState { get; set; }
        public string CI_Name { get; set; }
        public string CI_Description { get; set; }
        public string CI_Class { get; set; }
        public DateTime CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public DateTime CI_LastUpdateDate { get; set; }
        public object Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public List<SPFFileComposition12> SPFFileComposition_21 { get; set; }
        public List<SCLBSubmittalDocumentVersions12> SCLBSubmittalDocumentVersions_12 { get; set; }

    }
    public class SPFFileComposition12
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string CI_Name { get; set; }
        public string CI_Description { get; set; }
        public string CI_Class { get; set; }
        public DateTime CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public DateTime CI_LastUpdateDate { get; set; }
        public object Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }

    }
    public class SCLBSubmittalDocumentVersions12
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string CI_Name { get; set; }
        public string CI_Description { get; set; }
        public string CI_Class { get; set; }
        public DateTime CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public DateTime CI_LastUpdateDate { get; set; }
        public object Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public Boolean MPLDocRejected { get; set; }

        public SPFRevisionVersions_21 SPFRevisionVersions_21 { get; set; }



    }
    public class SPFRevisionVersions_21
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string CI_Name { get; set; }
        public string CI_Description { get; set; }
        public string CI_Class { get; set; }
        public DateTime CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public DateTime CI_LastUpdateDate { get; set; }
        public object Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }

    }
   
    public class DeserializeFilterQuery
    {
        public List<DeserializeOBID> value { get; set; }
    }
    public class DeserializeOBID
    {
        public string OBID;
    }
}

  
   