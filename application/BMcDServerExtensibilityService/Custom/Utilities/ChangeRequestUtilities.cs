using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Linq;
using static MPLServerExtensibilityService.Custom.Models.ChangeRequestModels;
using static MPLServerExtensibilityService.Custom.Models.HelperModels;
using static MPLServerExtensibilityService.SDxConstants;

namespace MPLServerExtensibilityService.Custom.Utilities
{
    public class ChangeRequestUtilities
    {
        HelperRepository helper;
        public ChangeRequestUtilities(HelperRepository helperRepo)
        {
            helper = helperRepo;
        }

        public bool TryGetPrimaryClassification(SimpleObject changeRequestObj, ref Classification classification)
        {
            bool isClassificationFound = false;
            try
            {
                string relatedPrimaryClassification = helper.GetRelatedObjects(ChangeRequestPrimaryClassificationRel, changeRequestObj.OBID);
                Classification[] classifications = { };
                classifications = JsonConvert.DeserializeObject<Classification[]>(relatedPrimaryClassification);
                if (classifications.Length > 0)
                {
                    Log.Information(classifications.Length + " no of related primary classification found for change request " + changeRequestObj.Name);
                    classification = classifications[0];
                    Log.Information("change request " + changeRequestObj.Name + " is related to primary classification " + classification.Name);
                    isClassificationFound = true;
                }
                else
                {
                    Log.Information("change request " + changeRequestObj.Name + " doesnt have primary classification");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in TryGetPrimaryClassification.");
            }
            return isClassificationFound;
        }

        public bool TryGetRelatedAsset(SimpleObject changeRequestObj, ref Asset asset)
        {
            bool isAssetFound = false;
            try
            {
                //string relatedAssetResult = helper.GetRelatedObjects("SDAChangeRequestAreas_12", changeRequestObj.OBID, "PTTArea");
                string relatedAssetResult = helper.GetRelatedObjects(ChangeRequestAssetRel, changeRequestObj.OBID);
                Asset[] assets = { };
                assets = JsonConvert.DeserializeObject<Asset[]>(relatedAssetResult);
                if (assets.Length > 0)
                {
                    Log.Information(assets.Length + " no of related assets found for change request " + changeRequestObj.Name);
                    asset = assets[0];
                    Log.Information("change request " + changeRequestObj.Name + " is related to asset " + asset.Name);
                    isAssetFound = true;
                }
                else
                {
                    Log.Information("change request " + changeRequestObj.Name + " doesnt have any related assets");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in TryGetRelatedAsset.");
            }
            return isAssetFound;
        }

        public int GetExistingHighestCRSequence()
        {
            // Assuming sequence is unique across the classifications and assets,
            // Will fetch all CR name props and find the existing highest sequence in the system

            int sequence = 0;
            try
            {
                // get the highest sequence by checking all CRs 1000 at a time as odata is returning max 1000 records
                string url = string.Format("Objects?$filter=Class eq '" + ChangeRequestClass + "'&$select=" + ChangeRequestIdProperty);// TBD: replace with ID property later
                bool fetchNextRecords = false;
                do
                {
                    string existingCRNamesResponse = helper.QueryData(url, "", 1000);
                    ObjectResponse resultObject = JsonConvert.DeserializeObject<ObjectResponse>(existingCRNamesResponse);
                    if (resultObject.value != null && resultObject.value.Count > 0)
                    {
                        int currHighestsequence = resultObject.value.
                            Where((JsonObject crName) =>
                            {
                                return crName[ChangeRequestIdProperty] != null;
                            }).
                              Select((JsonObject crName) =>
                              {
                                  //SimpleObject crName = (SimpleObject)crNameObj;
                                  return int.Parse(Convert.ToString(crName[ChangeRequestIdProperty]).Substring(Convert.ToString(crName[ChangeRequestIdProperty]).LastIndexOf("-") + 1));
                              }).
                              OrderByDescending(crSeqVal => crSeqVal).
                              FirstOrDefault();

                        sequence = (sequence < currHighestsequence) ? currHighestsequence : sequence;
                    }
                    else
                    {
                        Log.Information("Couldn't find existing change request IDs");
                    }

                    if (!string.IsNullOrWhiteSpace(resultObject.nextItemsLink))
                    {
                        url = resultObject.nextItemsLink;
                    }

                } while (fetchNextRecords);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetExistingHighestCRSequence.");
            }
            return sequence;
        }

        public string GetNewModificationID(SimpleObject modObj)
        {
            string modID = "";

            try
            {
                Asset asset = null;
                Classification classification = null;

                // Fetch Asset
                bool isAssetFound = TryGetRelatedAsset(modObj, ref asset);
                if (isAssetFound)
                {
                    modID = asset.Name;
                }

                // Fetch classification
                bool isClassificationFound = TryGetPrimaryClassification(modObj, ref classification);
                if (isClassificationFound)
                {
                    modID += "-" + classification.Name;//TBD: replace with ClassificationShortID once we configure it
                }

                // Fetch existing highest sequence and generate next sequence
                modID += "-" + (GetExistingHighestCRSequence() + 1).ToString().PadLeft(5, '0');

                Log.Information("New modification ID is " + modID);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetNewModificationID.");
            }

            return modID;
        }

        public SimpleObject CreateProjectForModification(SimpleObject modObj)
        {
            SimpleObject project = null;
            try
            {
                SimpleObject configStatus = null;
                SimpleObject plant = null;

                // Fetch created project status
                string configStatusResponse = helper.GetObjectByNameAndClass(ProjectStatusDefaultValue, ProjectStatusClass);
                ObjectResponse configStatusResponseObj = JsonConvert.DeserializeObject<ObjectResponse>(configStatusResponse);
                if (configStatusResponseObj.value != null && configStatusResponseObj.value.Count > 0)
                {
                    configStatus = (SimpleObject)configStatusResponseObj.value[0];
                }

                // Fetch parent config - i.e. plant
                string plantResponse = helper.GetObjectByNameAndClass(ProjectPlantName, ProjectPlantClass); //TBD: fetch from config file
                ObjectResponse plantResponseObj = JsonConvert.DeserializeObject<ObjectResponse>(plantResponse);
                if (plantResponseObj.value != null && plantResponseObj.value.Count > 0)
                {
                    plant = (SimpleObject)plantResponseObj.value[0];
                }

                // prepare project data - i.e Name,status,parent,related modification
                ProjectForCreation projToCreate = new ProjectForCreation();
                projToCreate.Class = ProjectClass;
                projToCreate.Name = "PRJ_" + modObj.Name;
                projToCreate.ConfigStatus = helper.GenerateUrlForRel(configStatus.OBID);
                projToCreate.Parent = helper.GenerateUrlForRel(plant.OBID);
                projToCreate.Modification = helper.GenerateUrlForRel(modObj.OBID);

                // send request to server
                string projCreationRespose = helper.CreateObject("Objects", modObj.Config, JsonConvert.SerializeObject(projToCreate));
                ObjectResponse resultObject = JsonConvert.DeserializeObject<ObjectResponse>(projCreationRespose);
                if (resultObject != null)
                {
                    if (resultObject.value != null && resultObject.value.Count > 0)
                    {
                        project = (SimpleObject)resultObject.value[0];
                    }
                    else
                    {
                        project = JsonConvert.DeserializeObject<SimpleObject>(projCreationRespose);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreateProjectForModification.");
            }

            return project;
        }
    }
}
