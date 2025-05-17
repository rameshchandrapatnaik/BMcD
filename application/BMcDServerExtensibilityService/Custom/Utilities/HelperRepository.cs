using BMcDExtensibilityService.Core;
using BMcDExtensibilityService.Core.EventPayload;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static BMcDExtensibilityService.Custom.Models.HelperModels;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Utilities
{
    public class HelperRepository
    {
        private ExtensibilityRestClient _restClient;
        private ExtensibilityConfiguration _config;
        public HelperRepository(ExtensibilityRestClient extensibilityRestClient, ExtensibilityConfiguration config)
        {
            _restClient = extensibilityRestClient;
            _config = config;
        }

        public ExecuteReportResponse ExecuteReport(string targetOBID, string reportName)
        {
            var apiUrl = String.Format("Reports('{0}')/Intergraph.SPF.Server.API.Model.ExecuteReport", reportName);

            var executeReportReqBody = new ExecuteReportRequest()
            {
                CSVSupported = true,
                Parameters = new List<string>(),
                SourceObjectIds = new List<string>() { targetOBID }
            };

            var executeReportRequest = new RestRequest(apiUrl);
            executeReportRequest.AddJsonBody(executeReportReqBody);

            var executeReportResponse = _restClient.restClient.Post(executeReportRequest);
            return JsonConvert.DeserializeObject<ExecuteReportResponse>(executeReportResponse.Content);
        }

        public byte[] DownloadExecutedReport(string reportUrl)
        {
            byte[] result;
            using (WebClient webClient = new WebClient())
            {
                result = webClient.DownloadData(reportUrl);
            }
            return result;
        }

        public SimpleObject GetObjectByOBID(string obid)
        {
            Log.Verbose("Begin: GetObjectByOBID");
            SimpleObject obj = null;
            try
            {
                var apiUrl = String.Format("Objects('{0}')", obid);
                var req = new RestRequest(apiUrl);

                var resp = _restClient.restClient.Get(req);

                if (resp.IsSuccessful)
                {
                    obj = JsonConvert.DeserializeObject<SimpleObject>(resp.Content);
                }
                else
                {
                    Log.Information("Server request failed: " + resp.Content);
                }
                Log.Verbose("End: GetObjectByOBID");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetObjectByOBID.");
            }
            return obj;

        }

        public bool AttachToDefaultWorkflow(string objOBID, string workflowName, string user)
        {
            Log.Verbose("Begin: AttachToDefaultWorkflow");

            var apiUrl = "AttachToDefaultWorkflow";

            var body = new AttachWorkflowRequest()
            {
                ObjectOBID = objOBID,
                WorkflowNameOrUID = workflowName
            };

            var request = new RestRequest(apiUrl);
            request.AddJsonBody(body);

            var resp = _restClient.restClient.Post(request);

            Log.Verbose("End: AttachToDefaultWorkflow");

            return resp.IsSuccessful;
        }


        public string GetObjectByNameAndClass(string name, string className, string columnSetName = null)
        {
            Log.Verbose("Begin: GetObjectByNameAndClass");

            var apiUrl = String.Format("Objects?$filter=Class eq '{0}' and Name eq '{1}'", className, name);
            var req = new RestRequest(apiUrl);

            if (columnSetName != null)
            {
                req.AddHeader("Accept", "application/vnd.intergraph.columnset." + columnSetName + "+json");
            }

            var resp = _restClient.restClient.Get(req);

            if (!resp.IsSuccessful)
            {
                Log.Information("Server request failed: " + resp.Content);
                //TODO: Write log and error stuff
            }
            Log.Verbose("End: GetObjectByNameAndClass");

            return resp.Content;
        }

        public string GetObjectByClassAndUID(string uidForFilter, string className, string columnSetName = null)
        {
            var apiUrl = String.Format("Objects?$filter=Class eq '{0}' and UID eq '{1}'", className, uidForFilter);
            var req = new RestRequest(apiUrl);

            if (columnSetName != null)
            {
                req.AddHeader("Accept", "application/vnd.intergraph.columnset." + columnSetName + "+json");
            }

            var resp = _restClient.restClient.Get(req);

            if (!resp.IsSuccessful)
            {
                //TODO: Write log and error stuff
            }
            return resp.Content;
        }

        public List<string> GenerateUrlForRel(string obid)
        {
            Log.Verbose("Begin: GenerateUrlForRel");

            var relList = new List<string>();

            relList.Add(String.Format("{0}/Objects('{1}')", _config.ServerBaseUri, obid));

            Log.Verbose("End: GenerateUrlForRel");
            return relList;
        }

        public async void SignoffStepAsync(ProcessStepExtensibilityEventPayload payload, string MessageToNextStep = "")
        {
            Log.Verbose("Begin: SignoffStepAsync");
            SignOffPayload lobjsignOffPayload = new SignOffPayload()
            {
                stepOBIDs = new List<string>() { payload.OBID },
                messageToNextStep = MessageToNextStep,
                signOffComment = "Signed off from extensibility"
            };

            string lstrRequestUri = "User/Intergraph.SPF.Server.API.Model.SignOffWorkflowStep()";

            var request = new RestRequest(lstrRequestUri, Method.POST);
            request.AddJsonBody(lobjsignOffPayload);

            request.AddHeader("SPFConfigUID", payload.ConfigUID);

            request.AddHeader("Accept", "application/vnd.intergraph.data+json");
            var lobjSignoffResponse = await _restClient.restClient.ExecuteAsync(request);

            if (lobjSignoffResponse.IsSuccessful)
            {
                Log.Information("Successfully signed off the step");
            }
            else
            {
                Log.Information("Server request failed: " + lobjSignoffResponse.Content);
            }
            Log.Verbose("End: SignoffStepAsync");
        }

        public string QuerySPFData(string pstrURL, string pstrConfigUID, string pstrAcceptParameter = "")
        {
            string lstrResponse = string.Empty;

            try
            {
                var request = new RestRequest(pstrURL, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-Ingr-OnBehalfOf", "SDxAdmin");
				request.AddHeader("SPFResolveDocumentsToVersion", "False");

				if (!string.IsNullOrEmpty(pstrAcceptParameter))
                    request.AddHeader("Accept", pstrAcceptParameter);
                else
                    request.AddHeader("Accept", "application/vnd.intergraph.data+json");
                request.AddHeader("spfconfiguid", pstrConfigUID);

                IRestResponse response = _restClient.restClient.Execute(request);
                lstrResponse = response.Content;

                if (!response.IsSuccessful)
                {
                    throw new Exception();
                }
                // Log.Information($"Response:  {lstrResponse}");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in QuerySPFData.");
            }
            return lstrResponse;
        }

        public string QueryData(string pstrURL, string pstrConfigUID, int pageSize = 0)
        {
            Log.Verbose("Begin: QueryData");
            string lstrReponse = "";
            try
            {
                var request = new RestRequest(pstrURL);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("spfconfiguid", pstrConfigUID);
                if (pageSize > 0)
                {
                    request.AddHeader("Prefer", string.Format("odata.maxpagesize={0}", pageSize));
                }
                IRestResponse response = _restClient.restClient.Get(request);
                lstrReponse = response.Content;
                Console.WriteLine($"Spfquerydata response:{lstrReponse}");
                Log.Information($"Spfquerydata response:{lstrReponse}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in QueryData.");
            }
            Log.Verbose("End: QueryData");
            return lstrReponse;
        }

        public string PatchUpdateSPFData(string pstrURL, string pstrConfigUID, string pstrBodytParameter = "", string pstrAcceptParameter = "")
        {
            Log.Verbose("Begin: PatchUpdateSPFData");
            string lstrReponse = string.Empty;
            try
            {
                var request = new RestRequest(pstrURL, Method.PATCH);
                request.AddHeader("Content-Type", "text/plain");
                if (!string.IsNullOrEmpty(pstrAcceptParameter))
                    request.AddHeader("Accept", pstrAcceptParameter);
                else
                    request.AddHeader("Accept", "application/vnd.intergraph.data+json");
                request.AddHeader("Prefer", "odata.omit-values=nulls");
                request.AddHeader("spfconfiguid", pstrConfigUID);
                request.AddParameter("text / plain", pstrBodytParameter, ParameterType.RequestBody);
                IRestResponse response = response = _restClient.restClient.Execute(request);

                //lstrReponse = response.Content;
                //Console.WriteLine(lstrReponse);
                if (!response.IsSuccessful)
                {
                    string lstrErrorMessage = $"PatchUpdateSPFData :: Web Request is not Successful. URL : {pstrURL} ResponseContent {response.Content} ErrorException {response.ErrorException} ErrorMessage {response.ErrorMessage}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception("Patch request failed.");
                }
                string lstrSuccessMessage = "PatchUpdateSPFData :: Web Request is not Successful.\\n";

                lstrReponse = response.Content;
            }
            catch
            {
                throw;
            }
            Log.Verbose("End: PatchUpdateSPFData");
            return lstrReponse;
        }

        public IRestResponse ExecutePostRequest(string pstrURL, string pstrConfigUID, string pstrRequestBody)
        {
            string lstrReponse;
            IRestResponse response = null;
            try
            {
                var request = new RestRequest(pstrURL);
                request.AddHeader("Accept", "application/json,application/vnd.intergraph.data+json");
                request.AddHeader("spfconfiguid", pstrConfigUID);
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("text/plain", pstrRequestBody, ParameterType.RequestBody);
                response = _restClient.restClient.Post(request);
                lstrReponse = response.Content;

                if (response.Content.Length > 201)
                {
                    Log.Information("Post Request Result  : STATUS CODE : " + response.StatusCode + "  CONTENT:" + response.Content.Substring(0, 200) + response.IsSuccessful);
                }
                else
                {
                    Log.Information("Post Request Result : STATUS CODE : " + response.StatusCode + " CONTENT:" + response.Content + response.IsSuccessful);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ExecutePostRequest.");
            }

            return response;
        }

        public string GetRelatedObjects(string RelDef, string objOBID, string columnSetName = "")
        {
            Log.Verbose("Begin: GetRelatedObjects");
            string RelatedObjJsonString = "";

            if (!string.IsNullOrWhiteSpace(RelDef))
            {
                var apiUrl = String.Format("Objects('{0}')/{1}", objOBID, RelDef);
                var req = new RestRequest(apiUrl);


                if (!string.IsNullOrWhiteSpace(columnSetName))
                {
                    req.AddHeader("Accept", "application/vnd.intergraph.columnset." + columnSetName + "+json");
                }

                var resp = _restClient.restClient.Get(req);

                if (!resp.IsSuccessful)
                {
                    Log.Error("Error in fetching related objects for " + RelDef);
                }
                else
                {
                    Log.Information("Received result from server for " + RelDef);
                    ObjectResponse resultObject = JsonConvert.DeserializeObject<ObjectResponse>(resp.Content);
                    if (resultObject.value == null || resultObject.value.Count <= 0)
                    {
                        RelatedObjJsonString = "[" + resp.Content + "]";
                    }
                    else
                    {
                        RelatedObjJsonString = JsonConvert.SerializeObject(resultObject.value);
                    }
                }
            }
            else
            {
                Log.Error("Reldef is empty string");
            }
            Log.Verbose("End: GetRelatedObjects");
            return RelatedObjJsonString;
        }

        public string CreateObject(string pstrURL, string pstrConfigUID, string pstrRequestBody)
        {
            Log.Verbose("Begin: CreateObject");
            string lstrReponse = string.Empty;

            try
            {
                var request = new RestRequest(pstrURL, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/vnd.intergraph.data+json");
                request.AddHeader("SPFConfigUID", pstrConfigUID);
                //request.AddHeader("X-Ingr-OnBehalfOf", payload.User);
                request.AddParameter("application/json", pstrRequestBody, ParameterType.RequestBody);
                IRestResponse response = response = _restClient.restClient.Execute(request);
                lstrReponse = response.Content;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreateObject");
            }
            Log.Verbose("End: CreateObject");
            return lstrReponse;
        }

        public int GetCountOfObjects(string pstrURL)
        {
            int count = 0;
            try
            {
                string urlToGetCount = pstrURL + string.Format("&$count=true");
                string existingCRNamesCountResponse = QueryData(urlToGetCount, "");
                if (!string.IsNullOrWhiteSpace(existingCRNamesCountResponse))
                {
                    ObjectResponse resultObject = JsonConvert.DeserializeObject<ObjectResponse>(existingCRNamesCountResponse);
                    if (!string.IsNullOrWhiteSpace(resultObject.count))
                    {
                        count = Convert.ToInt32(resultObject.count);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetCountOfObjects.");
            }
            return count;
        }
        public bool DeleteObject(string pstrURL, string pstrConfigUID)
        {
            Log.Verbose("Begin: Delete Object");
            //bool lstrReponse = string.Empty;

            try
            {
                var request = new RestRequest(pstrURL, Method.DELETE);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/vnd.intergraph.data+json");
                request.AddHeader("SPFConfigUID", pstrConfigUID);
                //request.AddHeader("X-Ingr-OnBehalfOf", payload.User);
                //request.AddParameter("application/json", pstrRequestBody, ParameterType.RequestBody);
                IRestResponse response = _restClient.restClient.Execute(request);
                if (!response.IsSuccessful)
                {
                    string lstrErrorMessage = $"DeleteObj :: Web Request is not Successful. URL : {pstrURL} ResponseContent {response.Content} ErrorException {response.ErrorException} ErrorMessage {response.ErrorMessage}";

                    Log.Error(lstrErrorMessage);
                }
                Log.Information("Delete Request Result : STATUS CODE : " + response.StatusCode + " CONTENT:" + response.Content + response.IsSuccessful);
                //lstrReponse = response.Content;
                Console.Write("Response Status - ", response.StatusCode);
                Console.Write("Response IsSuccessful - ", response.IsSuccessful);
                return response.IsSuccessful;
            }
            catch (Exception)
            {
                throw;
            }
            Log.Verbose("End: CreateObject");

        }

        public bool DeleteBatchRequest(string pstrURL, string pstrConfigUID, HashSet<string> OBIDs, string impersonateuser)
        {
            try
            {
                List<string> OBIDList = new List<string>(OBIDs);
                string lstrbaseurl = $"{pstrURL}/$batch";
                var batchRequest = new RestRequest(lstrbaseurl, Method.POST);
                string batchGuidValue = Guid.NewGuid().ToString();
                string lstrChnagesetguid = Guid.NewGuid().ToString();
                batchRequest.AddHeader("Content-Type", $"multipart/mixed; boundary=batch_{batchGuidValue}");
                batchRequest.AddHeader("X-Ingr-OnBehalfOf", impersonateuser);
                batchRequest.AddHeader("Accept", "application/vnd.intergraph.data+json");
                StringBuilder batchBody = new StringBuilder();
                batchBody.AppendLine($"--batch_{batchGuidValue}");
                batchBody.AppendLine($"Content-Type: multipart/mixed; boundary=changeset_{lstrChnagesetguid}");
                batchBody.AppendLine($"Content-Transfer-Encoding: binary");
                for (int i = 0; i < OBIDs.Count; i++)
                {
                    string lstrDeleteUri = $"{pstrURL}/SDA/Objects('{OBIDList[i]}')";


                    string lstrDeleteRequest = $@"
--changeset_{lstrChnagesetguid}
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: {i}

DELETE {lstrDeleteUri} HTTP/1.1
Accept: application/vnd.intergraph.data+json
spfconfiguid:{pstrConfigUID}
SPFDeleteTransition: Deleted
X-Ingr-OnBehalfOf: {impersonateuser}
";
                    batchBody.AppendLine();
                    lstrDeleteRequest.ToString();
                    batchBody.AppendLine(lstrDeleteRequest.Trim());
                    batchBody.AppendLine();
                    //batchBody.AppendLine($"--batch_{batchGuidValue}");

                    // Set batch body to request

                }
                Console.WriteLine(batchBody.ToString());
                Log.Information($"batch request body:::::{batchBody.ToString()}");
                batchRequest.AddParameter("text/plain", batchBody.ToString(), ParameterType.RequestBody);
                IRestResponse response = _restClient.restClient.Execute(batchRequest);
                Log.Information("Delete Request Result : STATUS CODE : " + response.StatusCode + " CONTENT:" + response.Content + response.IsSuccessful);
                Console.WriteLine(response.Content);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Batch Request Failed: {response.ErrorMessage}");
                }

                return true;
            }
            catch
            {
                throw;
            }


        }

        public bool DeleteRelBatchRequest(string pstrURL, string pstrConfigUID, List<String> plstRevisionOBID, List<String> plstTransmittalOBID, string impersonateuser)
        {
            try
            {

                string lstrbaseurl = $"{pstrURL}/$batch";
                var batchRequest = new RestRequest(lstrbaseurl, Method.POST);
                string batchGuidValue = Guid.NewGuid().ToString();
                string lstrChnagesetguid = Guid.NewGuid().ToString();
                batchRequest.AddHeader("Content-Type", $"multipart/mixed; boundary=batch_{batchGuidValue}");
                batchRequest.AddHeader("X-Ingr-OnBehalfOf", impersonateuser);
                batchRequest.AddHeader("Accept", "application/vnd.intergraph.data+json");


                StringBuilder batchBody = new StringBuilder();
                batchBody.AppendLine($"--batch_{batchGuidValue}");
                batchBody.AppendLine($"Content-Type: multipart/mixed; boundary=changeset_{lstrChnagesetguid}");
                batchBody.AppendLine($"Content-Transfer-Encoding: binary");
                for (int i = 0; i < plstRevisionOBID.Count; i++)
                {
                    string lstrDeleteUri = $"{pstrURL}/SDA/Objects('{plstRevisionOBID[i]}')/MPLDocumentRevTransmittal_12('{plstTransmittalOBID[i]}')/$ref";

                    string lstrDeleteRequest = @$"
--changeset_{lstrChnagesetguid}
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: {i}

DELETE {lstrDeleteUri} HTTP/1.1
Accept: application/vnd.intergraph.data+json
spfconfiguid:{pstrConfigUID}
SPFDeleteTransition: Deleted
X-Ingr-OnBehalfOf: {impersonateuser}
";
                    batchBody.AppendLine();
                    lstrDeleteRequest.ToString();
                    batchBody.AppendLine(lstrDeleteRequest.Trim());
                    batchBody.AppendLine();
                    //batchBody.AppendLine($"--batch_{batchGuidValue}");

                    // Set batch body to request

                }
                batchRequest.AddParameter("text/plain", batchBody.ToString(), ParameterType.RequestBody);
                Console.WriteLine(batchBody.ToString());
                Log.Information($"batch request body of reldelete request:::::{batchBody.ToString()}");
                IRestResponse response = _restClient.restClient.Execute(batchRequest);
                Log.Information("Delete Request Result : STATUS CODE : " + response.StatusCode + " CONTENT:" + response.Content + response.IsSuccessful);
                Console.WriteLine(response.Content);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Batch Request Failed: {response.ErrorMessage}");
                }

                return true;
            }
            catch
            {
                throw;
            }


        }

		//    public void SignoffStep(string pstrWorkflowStepOBID, string pstrConfig,string lstrURL, string pstrUser)
		//    {
		//        try
		//        {
		//            SignOffPayload lobjsignOffPayload = new SignOffPayload()
		//            {
		//                stepOBIDs = new List<string>() { pstrWorkflowStepOBID },
		//                signOffComment = "Signed off from extensibility",
		//                messageToNextStep = "Signed off from extensibility"
		//            };

		//            string lstrSignOffPayload = JsonConvert.SerializeObject(lobjsignOffPayload);

		//            //string lstrURL = "/SDA/User/Intergraph.SPF.Server.API.Model.SignOffWorkflowStep()";
		//            //string lstrURL = pstrURL;
		//RestRequest lRequest = new RestRequest(lstrURL, Method.POST);

		//            lRequest.AddHeader("Content-Type", "application/json");

		//            lRequest.AddHeader("Accept", "application/json, text/plain, */*");

		//            lRequest.AddJsonBody(lstrSignOffPayload);
		//lRequest.AddHeader("X-Ingr-OnBehalfOf", pstrUser);

		//IRestResponse lResponse = _restClient.restClient.Execute(lRequest);

		//            if (!lResponse.IsSuccessful)
		//            {
		//                string lstrErrorMessage = $"Approve Workflow Step :: Web Request is not Successful. URL : {lstrURL} ResponseContent {lResponse.Content} ErrorException {lResponse.ErrorException} ErrorMessage {lResponse.ErrorMessage}";

		//                Log.Error(lstrErrorMessage);

		//                throw new Exception(lstrErrorMessage);
		//            }


		//        }
		//        catch
		//        {
		//            throw;
		//        }

		//    }

		public void SignoffStep(ProcessStepExtensibilityEventPayload payload, string MessageToNextStep = "", string stepOBID = "")
		{
			Log.Verbose("Begin: SignoffStepAsync");
			//string lstrRequestUri = "User/Intergraph.SPF.Server.API.Model.SignOffWorkflowStep()";
			string lstrRequestUri = "/SDA/User/Intergraph.SPF.Server.API.Model.SignOffWorkflowStep()";
			try
			{
				var request = new RestRequest(lstrRequestUri, Method.POST);
				if (stepOBID != "")
				{
					SignOffPayload lobjsignOffPayload = new SignOffPayload()
					{
						stepOBIDs = new List<string>() { stepOBID },
						messageToNextStep = MessageToNextStep,
						signOffComment = "Signed off from extensibility"
					};
					request.AddJsonBody(lobjsignOffPayload);

				}
				else
				{
					SignOffPayload lobjsignOffPayload = new SignOffPayload()
					{
						stepOBIDs = new List<string>() { payload.OBID },
						messageToNextStep = MessageToNextStep,
						signOffComment = "Signed off from extensibility"
					};
					request.AddJsonBody(lobjsignOffPayload);

				}

				//request.AddJsonBody(lobjsignOffPayload);
				request.AddHeader("SPFConfigUID", payload.ConfigUID);

				request.AddHeader("Accept", "application/vnd.intergraph.data+json");
				var lobjSignoffResponse = _restClient.restClient.Execute(request);

				if (lobjSignoffResponse.IsSuccessful)
				{
					Log.Information("Step Signed off Successfully");
				}
				else
				{
					Log.Information("Sign off Failed:  " + lobjSignoffResponse.Content);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex + "error occurred while doing Step SignOff");
				throw;
			}
			Log.Verbose("End: SignoffStepAsync");
		}
		

		public void RejectStep(string pstrWorkflowStepOBID, string pstrConfig)
        {
            try
            {
                RejectPayload lobjsignOffPayload = new RejectPayload()
                {
                    stepOBIDs = new List<string>() { pstrWorkflowStepOBID },
                    rejectComment = "Rejected from extensibility",
                    messageToNextStep = "Rejected from extensibility"
                };

                string lstrSignOffPayload = JsonConvert.SerializeObject(lobjsignOffPayload);

                string lstrURL = "/SDA/User/Intergraph.SPF.Server.API.Model.RejectWorkflowStep()";

                RestRequest lRequest = new RestRequest(lstrURL, Method.POST);

                lRequest.AddHeader("Content-Type", "application/json");

                lRequest.AddHeader("Accept", "application/json, text/plain, */*");

                lRequest.AddJsonBody(lstrSignOffPayload);


                IRestResponse lResponse = _restClient.restClient.Execute(lRequest);

                if (!lResponse.IsSuccessful)
                {
                    string lstrErrorMessage = $"Approve Workflow Step :: Web Request is not Successful. URL : {lstrURL} ResponseContent {lResponse.Content} ErrorException {lResponse.ErrorException} ErrorMessage {lResponse.ErrorMessage}";

                    Log.Error(lstrErrorMessage);

                    throw new Exception(lstrErrorMessage);
                }

            }
            catch
            {
                throw;
            }

        }

        public void AttachWorkflow(string pstrWorkflowOBID, string pstrConfig, List<string> plstUserOBIDs, string pstrURL)
        {
            try
            {
                AttachWorkflowPayload lobjWorkflowPayload = new AttachWorkflowPayload()
                {
                    WorkflowObid = pstrWorkflowOBID,
                    ObjectObids = plstUserOBIDs
                };

                string lstrWorkflowPayload = JsonConvert.SerializeObject(lobjWorkflowPayload);

                RestRequest lRequest = new RestRequest(pstrURL, Method.POST);
                lRequest.AddHeader("Content-Type", "application/json");

                lRequest.AddHeader("Accept", "application/json, text/plain, */*");

                lRequest.AddJsonBody(lstrWorkflowPayload);

                IRestResponse lResponse = _restClient.restClient.Execute(lRequest);
                if (!lResponse.IsSuccessful)
                {
                    string lstrErrorMessage = $"Attach Workflow:: Web Request is not Successful. URL : {pstrURL} ResponseContent {lResponse.Content} ErrorException {lResponse.ErrorException} ErrorMessage {lResponse.ErrorMessage}";

                    Log.Error(lstrErrorMessage);

                    throw new Exception(lstrErrorMessage);
                }


            }
            catch
            {
                throw;
            }

        }


        #region
        public string RetrieveFileUris(string pstrURL)
        {
            Log.Verbose("Begin: file retrieve process");
            string lstrfileURIvalue = "";
            try
            {
                var request = new RestRequest(pstrURL, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/vnd.intergraph.data+json");

                IRestResponse response = response = _restClient.restClient.Execute(request);
                string lstrReponse = response.Content;
                FileUriResponse lstrfileURI = JsonConvert.DeserializeObject<FileUriResponse>(lstrReponse);
                lstrfileURIvalue = lstrfileURI.Value[0].Uri.ToString();
            }
            catch
            {
                throw;
            }
            Log.Verbose("End: File retrival file url for download the document");
            return lstrfileURIvalue;
        }



        public bool BatchCreateTransmittalDocRelObj(string pstrURL, string pstrlTransmittalOBID, List<String> plstRevisionOBIDS, string impersonateuser, string pstrConfigUID)
        {
            try
            {

                string lstrbaseurl = $"{pstrURL}/$batch";
                var batchRequest = new RestRequest(lstrbaseurl, Method.POST);
                string batchGuidValue = Guid.NewGuid().ToString();
                string lstrChnagesetguid = Guid.NewGuid().ToString();
                batchRequest.AddHeader("Content-Type", $"multipart/mixed; boundary=batch_{batchGuidValue}");
                batchRequest.AddHeader("X-Ingr-OnBehalfOf", impersonateuser);
                batchRequest.AddHeader("Accept", "application/vnd.intergraph.data+json");


                StringBuilder batchBody = new StringBuilder();
                batchBody.AppendLine($"--batch_{batchGuidValue}");
                batchBody.AppendLine($"Content-Type: multipart/mixed; boundary=changeset_{lstrChnagesetguid}");
                batchBody.AppendLine($"Content-Transfer-Encoding: binary");
                for (int i = 0; i < plstRevisionOBIDS.Count; i++)
                {
                    string lstrPatchUri = $"{pstrURL}/SDA/Objects('{plstRevisionOBIDS[i]}')";

                    string lstrPatchRequest = @$"
--changeset_{lstrChnagesetguid}
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: {i}

PATCH {lstrPatchUri} HTTP/1.1
Accept: application/vnd.intergraph.data+json
spfconfiguid:{pstrConfigUID}
X-Ingr-OnBehalfOf: {impersonateuser}
";
                    string lstrRel = @$"""MPLDocumentRevTransmittal_12@odata.bind"":[""{pstrURL}/SDA/Objects('{pstrlTransmittalOBID}')""]";
                    batchBody.AppendLine();
                    lstrPatchRequest.ToString();
                    batchBody.AppendLine(lstrPatchRequest.Trim());
                    batchBody.AppendLine();
                    batchBody.AppendLine("{");
                    batchBody.AppendLine(lstrRel.Trim());
                    batchBody.AppendLine("}");
                    batchBody.AppendLine();
                    //batchBody.AppendLine($"--batch_{batchGuidValue}");

                    // Set batch body to request

                }
                batchRequest.AddParameter("text/plain", batchBody.ToString(), ParameterType.RequestBody);
                Console.WriteLine(batchBody.ToString());
                IRestResponse response = _restClient.restClient.Execute(batchRequest);
                Log.Information("Create Transmittal Doc Rel Request Result : STATUS CODE : " + response.StatusCode + " CONTENT:" + response.Content + response.IsSuccessful);
                Console.WriteLine(response.Content);
                if (!response.IsSuccessful)
                {

                    throw new Exception($"Batch Request Failed: {response.ErrorMessage}");
                }

                return true;
            }
            catch
            {
                throw;
            }

        }
        #endregion


        public string GetLatestFile(string folderPath = null)
        {
            // Get all CSV files in the folder
            folderPath ??= _config.ReportFolderPath;
            var csvFiles = Directory.GetFiles(folderPath, "*.csv");

            // Check if there are any CSV files
            if (csvFiles.Length > 0)
            {
                // Get the latest CSV file by modification time
                var latestCsvFile = new DirectoryInfo(folderPath)
                    .GetFiles("*.csv")
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();

                return latestCsvFile.FullName;
            }
            else
            {
                return null;
            }
        }

        public string ModifyXfdf(string inputFilePath, string oldName, string newName)
        {
            //string lstrModifiedByte = "";
            try
            {
                XDocument xfdfDoc = XDocument.Load(inputFilePath);

                // Define the namespace used in the XFDF file
                XNamespace xfdfNamespace = "http://ns.adobe.com/xfdf/";

                // Find the element containing the base64 encoded string
                var stampElements = xfdfDoc.Descendants(xfdfNamespace + "trn-custom-data");

                foreach (var stampElement in stampElements)
                {
                    var bytesAttribute = stampElement.Attribute("bytes");

                    string decodedBytes = HttpUtility.HtmlDecode(bytesAttribute.Value);
                    Console.WriteLine(decodedBytes);
                    if (decodedBytes.Contains(oldName))
                    {
                        string modifiedBytes = decodedBytes.Replace(oldName, newName);

                        // Encode the modified content
                        string encodedBytes = HttpUtility.HtmlEncode(modifiedBytes);

                        // Update the bytes attribute
                        bytesAttribute.Value = encodedBytes;
                        
                    }
                    else
                    {
                        return null;
                    }
                }
                // Return the modified XFDF document as a string
                using (StringWriter stringWriter = new StringWriter())
                {
                    xfdfDoc.Save(stringWriter);
                    return stringWriter.ToString();
                }
            }
            catch
            {
                throw;
            }

        }



    }
}


