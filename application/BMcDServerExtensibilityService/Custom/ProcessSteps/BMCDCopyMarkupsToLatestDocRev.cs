using BMcDExtensibilityService.Core.EventPayload;
using BMcDExtensibilityService.Core;
using BMcDExtensibilityService.Custom.Utilities;
using Serilog;
using System;
using BMcDExtensibilityService.Custom.Models;
using Newtonsoft.Json;
using static BMcDExtensibilityService.Custom.Models.MarkupLayerModel;
using System.Linq;
using System.IO;

namespace BMcDExtensibilityService.CustomCode
{
	public class BMCDCopyMarkupsToLatestDocRev
	{

            public void Execute(ExtensibilityODataClient extensibilityODataClient, object body)
            {

                string lstrDesignFileOBID = null;
                //string newName = "Extensibility Stamp";
                HelperRepository helper = null;
                ProcessStepExtensibilityEventPayload payload = null;
                try
                {
                    Log.Information("Begin: AutoStampingBasedOnReturnCode - Execute");
					if (body == null)
					{
						throw new ArgumentNullException(nameof(body), "Body is null.");
					}
					ExtensibilityDeserializer<ProcessStepExtensibilityEventPayload> deserializer = new ExtensibilityDeserializer<ProcessStepExtensibilityEventPayload>();
                    payload = deserializer.DeserializeObject(body);
					if (payload == null || string.IsNullOrEmpty(payload.OBID))
					{
						throw new Exception("Deserialization failed: Payload or OBID is null");
					}
					if (string.IsNullOrEmpty(payload.ConfigUID))
					{
						throw new Exception("Payload ConfigUID is null or empty.");
					}

					if (string.IsNullOrEmpty(payload.WorkflowObjectObid))
					{
						throw new Exception("Payload WorkflowObjectObid is null or empty.");
					}
					if (string.IsNullOrEmpty(payload.User))
					{
						throw new Exception("Payload User is null or empty.");
					}
					Log.Information("Payload: " + Environment.NewLine + payload.OBID);

                    ExtensibilityRestClient extRestClient = new ExtensibilityRestClient(extensibilityODataClient.extensibilityConfiguration, extensibilityODataClient._authenticationService);
                    extRestClient.SetImpersonationHeader(payload.User);
				

					helper = new HelperRepository(extRestClient, extensibilityODataClient.extensibilityConfiguration);
                    FileRepository fileRepo = new FileRepository(extRestClient);





				string lstrOBID = payload.OBID;
				string lstrconfigUID = payload.ConfigUID;
				Log.Information($"Payload OBID:{lstrOBID}");
				string lstrDocRevObid = payload.WorkflowObjectObid;

				//Get the Workflow step status
				string lstrRequestGetWFStatusUri = $"{extRestClient.restClient.BaseUrl}/SDA/Objects('{lstrOBID}')?$select=SPFStepStatus";
				Log.Information($"Request URI:{lstrRequestGetWFStatusUri}");
				string lstrWFStepStatusResponse = helper.QuerySPFData(lstrRequestGetWFStatusUri, lstrconfigUID);
				if (string.IsNullOrEmpty(lstrWFStepStatusResponse))
				{
					throw new Exception("Failed to get workflow step status.");
				}

				WorkflowStep lstrWorkflowStep = JsonConvert.DeserializeObject<WorkflowStep>(lstrWFStepStatusResponse);
				if (lstrWorkflowStep is not null)
				{
					string lstrWorkflowStepStatus = lstrWorkflowStep.SPFStepStatus;
					if (lstrWorkflowStepStatus == "RS")
					{

						//Get the Master OBID
						string lstrRequestUri = $"{extRestClient.restClient.BaseUrl}/SDA/Objects('{lstrDocRevObid}')?$expand=SPFDocumentRevisions_21";
						Log.Information($"Request URI:{lstrRequestUri}");
						string lstrResponse = helper.QuerySPFData(lstrRequestUri, lstrconfigUID);
						if (string.IsNullOrEmpty(lstrResponse))
						{
							throw new Exception("Failed to retrieve master document.");
						}
						SPFDocRevision1 lstrMasterResBody = JsonConvert.DeserializeObject<SPFDocRevision1>(lstrResponse);

						string lstrMasterOBID = lstrMasterResBody?.SPFDocumentRevisions_21?.OBID;
						if (string.IsNullOrEmpty(lstrMasterOBID))
						{
							throw new Exception("Master OBID is null.");
						}
						string lstrRequestUri2 = $"{extRestClient.restClient.BaseUrl}/SDA/Objects('{lstrMasterOBID}')?$expand=SPFDocumentRevisions_12($expand=SPFRevisionVersions_12($filter=SPFIsDocVersionSuperseded eq false;$expand=SCLBDocVersionsMarkupFiles_12($expand=SPFItemOwningGroup_12),SPFFileComposition_21($expand=SPFFileFileType_12,SPFFileMarkup_12($expand=SPFItemOwningGroup_12),SPFFileMarkupRendition_12($expand=SPFFileMarkup_12($expand=SPFItemOwningGroup_12)))))";
						Log.Information($"Request URI:{lstrRequestUri2}");
						string lstrRevMarkupResponse = helper.QuerySPFData(lstrRequestUri2, lstrconfigUID);
						if (string.IsNullOrEmpty(lstrRevMarkupResponse))
						{
							throw new Exception("Failed to retrieve document revisions.");
						}
						FDWDocumentMaster lstrFDWDocMaster = JsonConvert.DeserializeObject<FDWDocumentMaster>(lstrRevMarkupResponse);

						var revisions = lstrFDWDocMaster?.SPFDocumentRevisions_12;

						if (revisions == null )
						{
							throw new Exception("Insufficient document revisions.");
						}

						int revCount = lstrFDWDocMaster.SPFDocumentRevisions_12.Count;

						if (revCount > 1)
						
						{


							SPFDocumentRevisions12 lstrCurrentRevision = lstrFDWDocMaster.SPFDocumentRevisions_12[revCount - 1];
							SPFDocumentRevisions12 lstrPreviousRevision = lstrFDWDocMaster.SPFDocumentRevisions_12[revCount - 2];
							// check if current and previous revision contain one non superseeded versions
							if (lstrCurrentRevision.SPFRevisionVersions_12 == null || !lstrCurrentRevision.SPFRevisionVersions_12.Any())
							{
								throw new Exception("Current revision does not contain any versions.");
							}
							if (lstrPreviousRevision.SPFRevisionVersions_12 == null || !lstrPreviousRevision.SPFRevisionVersions_12.Any())
							{
								throw new Exception("Previous revision does not contain any versions.");
							}

							//Get the count of Files


							int fileCount = lstrPreviousRevision.SPFRevisionVersions_12[0]?.SPFFileComposition_21?.Count ?? 0;
							if(fileCount > 0) {

								//string lstrVersionOBID = lstrCurrentRevision.SPFRevisionVersions_12[0].OBID;
								var prevFiles = lstrPreviousRevision.SPFRevisionVersions_12[0]?.SPFFileComposition_21;
								var curFiles = lstrCurrentRevision.SPFRevisionVersions_12[0]?.SPFFileComposition_21;

								if (prevFiles != null && curFiles != null)
								{

									foreach (var prevRevFile in prevFiles)
									{
										int prevRevmarkupCount = lstrPreviousRevision.SPFRevisionVersions_12[0]?.SCLBDocVersionsMarkupFiles_12?.Count ?? 0;
										// Count of selected markups in the previous revision. Only these need to be copied.

										lstrDesignFileOBID = "";

										foreach (var curRevFile in curFiles)
										{
											//int curRevmarkupCount = curRevFile.SPFFileMarkupRendition_12.SPFFileMarkup_12.Count;
											string lstrContextObjOBID = curRevFile.OBID;
											if (prevRevFile.Name == curRevFile.Name && prevRevmarkupCount > 0)
											{
												string SPFRemoteFileName = curRevFile?.SPFRemoteFileName;
												//string fileExtension = Path.GetExtension(SPFRemoteFileName);
												string fileExtension = curRevFile.SPFFileFileType_12[0].Name;
												if (string.IsNullOrEmpty(fileExtension))
												{
													Console.WriteLine("File does not have an extension.");
													throw new Exception("File does not have an extension.");
												}
												else
												{
													Console.WriteLine($"File extension: {fileExtension}");
												}

												if (fileExtension.Equals("pdf", StringComparison.OrdinalIgnoreCase))
												{
													lstrDesignFileOBID = curRevFile?.OBID;
												}
												else
												{
													lstrDesignFileOBID = curRevFile?.SPFFileMarkupRendition_12?.OBID ?? throw new Exception("SPFFileMarkupRendition doesnt exist for current revision file.");
												}


												//check for each consolidated markup markup of previous Revision file 
												var markupFiles = lstrPreviousRevision.SPFRevisionVersions_12[0]?.SCLBDocVersionsMarkupFiles_12;

												if (markupFiles != null)
												{
													//loop  for each consolidated markup markup of previous Revision file
													foreach (var markup in lstrPreviousRevision.SPFRevisionVersions_12[0].SCLBDocVersionsMarkupFiles_12)
													{
														// check if consolidated markup is part of previous revision files to copy to the same file present on the current revision
														bool isMarkupPresent=false;


														if (fileExtension.Equals("pdf", StringComparison.OrdinalIgnoreCase))
														{
															
															 isMarkupPresent = prevRevFile.SPFFileMarkup_12.Any(m => m.OBID == markup.OBID);
														}
														else
														{
															 isMarkupPresent = prevRevFile.SPFFileMarkupRendition_12.SPFFileMarkup_12.Any(m => m.OBID == markup.OBID);
														}
														if (isMarkupPresent)
														{
														string lstrRequestUri3 = $"{extRestClient.restClient.BaseUrl}/SDA/GetMarkupContent";
														Log.Information($"Request URI:{lstrRequestUri3}");
														string previousDescription = markup.Description;
														var reqBody = new { MarkupOBID = markup.OBID };
														var lstrPatchRes = helper.ExecutePostRequest(lstrRequestUri3, lstrconfigUID, JsonConvert.SerializeObject(reqBody));
															if (lstrPatchRes == null)
															{
																throw new Exception("Failed to get a valid response from ExecutePostRequest.");
															}

															MarkupContent MarkupContent = JsonConvert.DeserializeObject<MarkupContent>(lstrPatchRes.Content);
														
															string lstrMarkUpContent = MarkupContent?.value ?? string.Empty;
															string lstrSPFItemOwningGroup = markup?.SPFItemOwningGroup_12?.OBID;

															if (string.IsNullOrEmpty(lstrSPFItemOwningGroup))
															{
																Log.Warning("Skipping markup: SPFItemOwningGroup or OBID is null.");
																continue; // Skip the current iteration and move to the next item in the loop
															}
															
															
															string sdxPostReq = $"{extRestClient.restClient.BaseUrl}/SDA/Objects";

															
															MarkupFileObj MarkupFileReqBody = new MarkupFileObj()
																{
																	Class = "SPFMarkupFile",
																	Name = "TBA: To be allocated using ENS",
																	Description = "(Previous) - " + previousDescription,
																	SPFLocalFileName = "1720417133529.xfdf",
																	SPFIsMarkupConsolidated = "false",
																	SPFMarkupColor = "10",
																	SPFItemOwningGroup_12 = $"{extRestClient.restClient.BaseUrl}/SDA/Objects('{lstrSPFItemOwningGroup}')",
																	SPFFileMarkup_21 = $"{extRestClient.restClient.BaseUrl}/SDA/Objects('{lstrDesignFileOBID}')"


																};
															if (MarkupFileReqBody == null)
															{
																throw new Exception("MarkupFileReqBody is null.");
															}

															string lstrAcceptParameter = "application/vnd.intergraph.data+json";
															string lstrMarkupReqBody = JsonConvert.SerializeObject(MarkupFileReqBody);
															string lstrMarkupObjOBIDRes = fileRepo.CreateMarkupObj(sdxPostReq, lstrMarkupReqBody, lstrconfigUID, payload.User, lstrAcceptParameter);
															if (string.IsNullOrEmpty(lstrMarkupObjOBIDRes))
															{
																throw new Exception("Response for Markup Object OBID is null or empty.");
															}
															CreateMarkupObj lstrcreateMarkupObj = JsonConvert.DeserializeObject<CreateMarkupObj>(lstrMarkupObjOBIDRes);
															if (lstrcreateMarkupObj == null)
															{
																throw new Exception("Deserialization failed: CreateMarkupObj is null.");
															}

															if (string.IsNullOrEmpty(lstrcreateMarkupObj.OBID))
															{
																throw new Exception("CreateMarkupObj OBID is null or empty.");
															}

															string lstrMarkupObjOBID = lstrcreateMarkupObj.OBID;


															//    ******    create MarkUpLayer Object with the SPFMarkupFile object.    ******    //
															string sdxCreateMarkupURL = $"{extRestClient.restClient.BaseUrl}/SDA/CreateMarkup";
															CreateMarkUp CreateMarkupReqBody = new CreateMarkUp()
															{
																MarkupOBID = lstrMarkupObjOBID,
																MarkupContent = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><xfdf xmlns=\"http://ns.adobe.com/xfdf/\" xml:space=\"preserve\"><annots /></xfdf>",
																MarkupContextObjectOBID = "",
																RelDefFromContextObjectToMarkupFile = "",
																RelDefForRenditionGeneration = "",
																ObjectOBIDForRenditionGeneration = "",
																IsBatchRequest = false

															};
															if (CreateMarkupReqBody == null)
															{
																throw new ArgumentNullException(nameof(CreateMarkupReqBody), "CreateMarkupReqBody is null.");
															}

															string lstrCreateMarkupObjOBIDRes = fileRepo.CreateMarkupObj(sdxCreateMarkupURL, JsonConvert.SerializeObject(CreateMarkupReqBody), lstrconfigUID, payload.User);
															if (string.IsNullOrEmpty(lstrCreateMarkupObjOBIDRes))
															{
																throw new Exception("CreateMarkupObj response is null or empty.");
															}
															CreateMarkupObjRes lstrcreateMarkupsObj = JsonConvert.DeserializeObject<CreateMarkupObjRes>(lstrCreateMarkupObjOBIDRes);
															if (lstrcreateMarkupsObj == null)
															{
																throw new Exception("Deserialization failed: CreateMarkupObjRes is null.");
															}
															Log.Information("Markup object created successfully");

														//   ******   Save MarkUpLayer Object on the Design file.   ******   //
														Log.Information($"Save Markup API intiated");
														string sdxSaveMarkupURL = $"{extRestClient.restClient.BaseUrl}/SDA/SaveMarkup";
														SaveMarkup SaveMarkupReqBody = new SaveMarkup()
														{
															ActionInterface = "",
															ActionPinOBIDs = "",															
															MarkupContent = lstrMarkUpContent,
															MarkupOBID = lstrMarkupObjOBID,
															RelDefForActionRenditionGen = ""
														};
															if (SaveMarkupReqBody == null)
															{
																throw new ArgumentNullException(nameof(SaveMarkupReqBody), "SaveMarkupReqBody is null.");
															}
															string lstrSaveMarkupObjOBIDRes = fileRepo.SaveMarkupObj(sdxSaveMarkupURL, JsonConvert.SerializeObject(SaveMarkupReqBody), lstrconfigUID, payload.User);
															if (string.IsNullOrEmpty(lstrSaveMarkupObjOBIDRes))
															{
																throw new Exception("Failed to save markup object: Response is null or empty.");
															}
															SaveMarkupRes lstrsaveMarkupsObj = JsonConvert.DeserializeObject<SaveMarkupRes>(lstrSaveMarkupObjOBIDRes);
															if (lstrsaveMarkupsObj == null)
															{
																throw new Exception("Deserialization failed: lstrsaveMarkupsObj is null.");
															}
															Console.WriteLine($"Save Markup is completed successfully - value {lstrsaveMarkupsObj.value}");
															Log.Information($"Save Markup is completed successfully - value {lstrsaveMarkupsObj.value}");

													}
												}
											}

											}

										}
									}
								}// check for previous revision files and current revision files.

						}//check for file count of previous revision

					}//check the count of rev

						string lstrURL = $"{extRestClient.restClient.BaseUrl}/SDA/User/Intergraph.SPF.Server.API.Model.SignOffWorkflowStep()";
						//helper.SignoffStep(payload.OBID, payload.ConfigUID,lstrURL, payload.User);
						helper.SignoffStep(payload);
					}
				}//check=RS
				

			}
                catch (Exception ex)
                {
                    Log.Error($"Exception occurred while executing AutoStamp on consolidated document WF Step.{ex.Message} -- {ex.StackTrace}");
                    if (helper != null && payload != null)
                    {
                        helper.RejectStep(payload.OBID, payload.ConfigUID);
                    }
                }
            }



        }
    
}
