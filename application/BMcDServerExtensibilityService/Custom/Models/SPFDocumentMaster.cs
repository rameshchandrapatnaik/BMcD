using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Models
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class SPFDocRevision1
	{
		[JsonProperty("@odata.context")]
		public string odatacontext { get; set; }
		public string UID { get; set; }
		public string Class { get; set; }
		public string Name { get; set; }
		public string OBID { get; set; }
		
		public string Description { get; set; }
		public DateTime TerminationDate { get; set; }
		public DateTime SPFRevIssueDate { get; set; }
		public bool SPFRevUnderChangeInSameConfig { get; set; }
		public string SPFMinorRevision { get; set; }
		public string SPFMajorRevision { get; set; }
		public string SPFRevState { get; set; }
		public object SPFCollaboratingOrganizations { get; set; }
		public object SPFMergedFromConfig { get; set; }
		public object SPFMergeDate { get; set; }
		public SPFDocumentRevisions21 SPFDocumentRevisions_21 { get; set; }
	}

	public class SPFDocumentRevisions21
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string UniqueKey { get; set; }
		public object Description { get; set; }
		public string OriginatingOrgName { get; set; }
		public string SPFDocState { get; set; }
		public string SecurityCodeName { get; set; }
		public string SDADocCategory { get; set; }
	}


	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class FDWDocumentMaster
	{
		[JsonProperty("@odata.context")]
		public string odatacontext { get; set; }
		public string UID { get; set; }
		public string Class { get; set; }
		public string Name { get; set; }
		public string OBID { get; set; }
		public string SDADocCategory { get; set; }
		public object SDADocType { get; set; }
		public object SDADocSubType { get; set; }
		public object SDADocSheetNumber { get; set; }
		public string SPFDocState { get; set; }
		public object SPFDocSubtype { get; set; }
		public object SPFDocCategory { get; set; }
		public object SPFTitle { get; set; }
		public object SPFDocType { get; set; }
		public object ENSRefName { get; set; }
		public object ENSSequenceNumber { get; set; }
		public object SDAReviewHoldStatus { get; set; }
		public string SecurityCodeName { get; set; }
		public object SPFNDocTransferStatus { get; set; }
		public object SPFNAreDocRelsValidated { get; set; }
		public object SPFNFDWDocMasterRevScheme { get; set; }
		public object SPFNPreventCDT { get; set; }
		public object SPFDocCancelledDate { get; set; }
		public object SPFDocCancelledBy { get; set; }
		public object SPFDocReasonForCancel { get; set; }
		public object SDxSource { get; set; }
		public object Description { get; set; }
		public DateTime TerminationDate { get; set; }
		public object SDxExternalVersion { get; set; }
		public DateTime CreationDate { get; set; }
		public object ClaimedToConfigs { get; set; }
		public string CreationUser { get; set; }
		public string DomainUID { get; set; }
		public string OriginatingOrgName { get; set; }
		public string Config { get; set; }
		public object SDxInternalVersion { get; set; }
		public object SDxByProduct { get; set; }
		public object TerminationUser { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public string UniqueKey { get; set; }
		public object ContainerID { get; set; }
		public object SDxByCustomer { get; set; }
		public List<SPFDocumentRevisions12> SPFDocumentRevisions_12 { get; set; }
	}

	public class SPFDocumentRevisions12
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string UniqueKey { get; set; }
		public string Description { get; set; }
		public string OriginatingOrgName { get; set; }
		public string SPFMajorRevision { get; set; }
		public string SPFExternalRevision { get; set; }
		public string SPFMinorRevision { get; set; }
		public DateTime SPFRevIssueDate { get; set; }
		public object SPFCollaboratingOrganizations { get; set; }
		public bool SPFRevUnderChangeInSameConfig { get; set; }
		public string SPFRevState { get; set; }
		public List<SPFRevisionVersions12> SPFRevisionVersions_12 { get; set; }
		public int? SPFActiveWorkflowCount { get; set; }
		public string SPFActiveWorkflowStatus { get; set; }
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string Description { get; set; }
		public string OriginatingOrgName { get; set; }
		public bool SPFNeedsIndexing { get; set; }
		public bool SPFIsFileCheckedOut { get; set; }
		public bool SPFViewInd { get; set; }
		public string SPFLocalHostName { get; set; }
		public string SPFRemoteFileName { get; set; }
		public string SPFLocalFileName { get; set; }
		public string SPFFileSize { get; set; }
		public List<SPFFileFileType12> SPFFileFileType_12 { get; set; }
		public List<SPFFileMarkup12> SPFFileMarkup_12 { get; set; }
		public SPFFileMarkupRendition12 SPFFileMarkupRendition_12 { get; set; }
	}
	public class SPFFileFileType12
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
	public class SPFFileMarkupRendition12
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string Description { get; set; }
		public object SPFLocalHostName { get; set; }
		public string SPFRemoteFileName { get; set; }
		public string SPFLocalFileName { get; set; }
		public string SPFFileSize { get; set; }
		public List<SPFFileMarkup12> SPFFileMarkup_12 { get; set; }
	}

	public class SPFFileMarkup12
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string Description { get; set; }
		public int SPFMarkupColor { get; set; }
		public string SPFMarkupType { get; set; }
		public bool SPFIsMarkupConsolidated { get; set; }
		public string SPFMarkupText { get; set; }
		public string SPFLocalHostName { get; set; }
		public string SPFRemoteFileName { get; set; }
		public string SPFLocalDirectory { get; set; }
		public string SPFLocalFileName { get; set; }
		public SPFItemOwningGroup12 SPFItemOwningGroup_12 { get; set; }
	}

	public class SPFItemOwningGroup12
	{
		public string UID { get; set; }
		public string Class { get; set; }
		public string Name { get; set; }
		public string OBID { get; set; }
		public object Config { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public DateTime CreationDate { get; set; }
		public string CreationUser { get; set; }
		public DateTime TerminationDate { get; set; }
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string SDxInternalVersion { get; set; }
		public string SDxSource { get; set; }
	}
	public class SPFRevisionVersions12
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string UniqueKey { get; set; }
		public string Description { get; set; }
		public string OriginatingOrgName { get; set; }
		public bool SPFNeedsIndexing { get; set; }
		public int SPFDocVersion { get; set; }
		public bool SPFIsDocVersionSuperseded { get; set; }
		public List<SCLBDocVersionsMarkupFiles12> SCLBDocVersionsMarkupFiles_12 { get; set; }
		public List<SPFFileComposition21> SPFFileComposition_21 { get; set; }
	}

	public class MarkupContent
	{
		[JsonProperty("@odata.context")]
		public string odatacontext { get; set; }
		public string value { get; set; }
	}


	public class WorkflowStep
	{
		[JsonProperty("@odata.context")]
		public string odatacontext { get; set; }
		public string SPFStepStatus { get; set; }
	}
	public class SCLBDocVersionsMarkupFiles12
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
		public object TerminationUser { get; set; }
		public string DomainUID { get; set; }
		public string Description { get; set; }
		public int SPFMarkupColor { get; set; }
		public string SPFMarkupType { get; set; }
		public bool SPFIsMarkupConsolidated { get; set; }
		public string SPFMarkupText { get; set; }
		public string SPFLocalHostName { get; set; }
		public string SPFRemoteFileName { get; set; }
		public string SPFLocalDirectory { get; set; }
		public string SPFLocalFileName { get; set; }

		public SPFItemOwningGroup12 SPFItemOwningGroup_12 { get; set; }
		public object ContainerID { get; set; }
	}


}
