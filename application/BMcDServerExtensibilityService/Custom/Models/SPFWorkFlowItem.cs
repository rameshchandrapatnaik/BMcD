using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Models
{
    /*
    public class SPFWorkFlowItem
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
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
        public SPFWorkflowWorkflowStep21 SPFWorkflowWorkflowStep_21 { get; set; }
    }

    public class SDADocumentRenditions12
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string CI_Name { get; set; }
        public object CI_Description { get; set; }
        public string CI_Class { get; set; }
        public DateTime CI_CreationDate { get; set; }
        public string CI_CreatedBy { get; set; }
        public DateTime CI_LastUpdateDate { get; set; }
        public string Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<SPFRenditionFileComposition21> SPFFileComposition_21 { get; set; }
    }

    public class SPFRenditionFileComposition21
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
        public string Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public string SPFFileSize { get; set; }
    }

    public class SPFItemWorkflow21
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
        public string SDAReturnCode { get; set; }
        public string Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<SCLBDocRevReviews12> SCLBDocRevReviews_12 { get; set; }
        public List<SPFRevisionVersions12> SPFRevisionVersions_12 { get; set; }
        public SPFDocumentRevisions21 SPFDocumentRevisions_21 { get; set; }
        public List<SDADocumentRenditions12> SDADocumentRenditions_12 { get; set; }
    }

    public class SPFWorkflowWorkflowStep21
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
        public SPFItemWorkflow21 SPFItemWorkflow_21 { get; set; }
    }
    public class SCLBDocRevReviews12
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string Config { get; set; }
    }
    public class SPFDocumentRevisions2111
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public SPFPrimaryClassification21 SPFPrimaryClassification_21 { get; set; }
    }
    
    public class SPFPrimaryClassification21
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
    }
    public class SPFRevisionVersions12
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public List<SPFFileComposition21> SPFFileComposition_21 { get; set; }
    }
    public class SPFFileComposition21
    {
        public string UID { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string OBID { get; set; }
        public string Config { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime TerminationDate { get; set; }
        public string DomainUID { get; set; }
        public string OriginatingOrgName { get; set; }
        public bool SPFNeedsIndexing { get; set; }
        public bool SPFIsFileCheckedOut { get; set; }
        public bool SPFViewInd { get; set; }
        public bool SPFEditInd { get; set; }
        public string SPFRemoteFileName { get; set; }
        public string SPFLocalFileName { get; set; }
        public string SPFLocalDirectory { get; set; }
        public string SPFFileSize { get; set; }
    }
    */
    
}
