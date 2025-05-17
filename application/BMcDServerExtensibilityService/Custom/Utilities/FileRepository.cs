using Amqp;
using BMcDExtensibilityService.Core;
using Newtonsoft.Json;
using RestSharp;
using RILExtensibilityService.Infra.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using static BMcDExtensibilityService.Custom.Models.FileModel;

namespace BMcDExtensibilityService.Custom.Utilities
{
    public class FileRepository
    {
        private ExtensibilityRestClient _restClient;

        public FileRepository(ExtensibilityRestClient extensibilityRestClient)
        {
            _restClient = extensibilityRestClient;
        }

        //public FileUploadResponse UploadFile(FileRequest file, string configUID, string pstrUser)
        //{
        //    try
        //    {
        //        string apiUploadUrl = $"{_restClient.restClient.BaseUrl}/FileMgmt/UploadFile";
        //        var fileUploadRequest = new RestRequest(apiUploadUrl, Method.POST);
        //        fileUploadRequest.AddFileBytes(file.FileName, file.FileContent, file.FileName);

        //        if (configUID != null)
        //        {
        //            fileUploadRequest.AddHeader("spfconfiguid", configUID);
        //        }
        //        fileUploadRequest.AddHeader("X-Ingr-OnBehalfOf", pstrUser);

        //        var fileUploadResponse = _restClient.restClient.Execute(fileUploadRequest);

        //        if (!fileUploadResponse.IsSuccessful)
        //        {
        //            string lstrErrorMessage = $"UploadFile : File Upload Request Not Successful. URL {apiUploadUrl} Response: {fileUploadResponse.Content} ResponseErrorMessage {fileUploadResponse.ErrorMessage} ResponseErrorException {fileUploadResponse.ErrorException}";
        //            Log.Error(lstrErrorMessage);
        //            throw new Exception(lstrErrorMessage);
        //        }

        //        return JsonConvert.DeserializeObject<FileUploadResponse>(fileUploadResponse.Content);
        //    }
        //    catch 
        //    {
        //        throw;
        //    }
        //}

        public bool UploadFile(string file, string directoryId, string pstrUserName, string pstrConfigUID, string pstrTargetOBIDtoAttach = null)
        {
            try
            {
                FileUploadResponse uploadFileResp = UploadLargeFile(file, pstrUserName);

                Log.Debug($"File Upload ID {uploadFileResp.UploadId}");

                MakeUploadAvailable(uploadFileResp.UploadId, pstrUserName);

                var uploadResp = CheckUploadStatus(uploadFileResp.UploadId, pstrUserName);

                if (pstrTargetOBIDtoAttach != null)
                {
                    AttachFile(uploadFileResp.UploadId, file, pstrTargetOBIDtoAttach, pstrConfigUID);
                }

                return MoveUploadedFileToDirectory(uploadFileResp.UploadId, Path.GetFileName(file), directoryId, pstrUserName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public FileUploadResponse UploadLargeFile(string file, string pstrUserName)
        {
            try
            {
                //split the file into Parts and Get the Paths for the File Parts
                List<string> fileParts = SplitLargeFileintoChunks(file, 5);

                Log.Information($"UploadLargeFile: File {file} is splitted into no of Parts {fileParts.Count()}");

                var fileName = Path.GetFileName(file);

                //Upload First Part
                string apiUploadUrlPart1 = "/FileMgmt/UploadFile?type=resume&part=1";

                var fileUploadRequest = new RestRequest(apiUploadUrlPart1);

                fileUploadRequest.AddFileBytes(fileName, FileHelper.GetFileContent(fileParts[0]), fileName);

                fileUploadRequest.AddHeader("X-Ingr-OnBehalfOf", pstrUserName);

                var fileUploadResponsePart1 = _restClient.restClient.Post(fileUploadRequest);

                if (!fileUploadResponsePart1.IsSuccessful)
                {
                    string lstrErrorMessage = $"File Upload Request Not Successful. URL {apiUploadUrlPart1} Response: {fileUploadResponsePart1.Content} ResponseErrorMessage {fileUploadResponsePart1.ErrorMessage} ResponseErrorException {fileUploadResponsePart1.ErrorException}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                var part1Result = JsonConvert.DeserializeObject<FileUploadResponse>(fileUploadResponsePart1.Content);

                //Upload Remaining Parts of the file
                for (int i = 1; i < fileParts.Count; i++)
                {
                    var apiParts = String.Format("/FileMgmt/UploadFile?type=resume&part={0}&UploadId={1}", i + 1, part1Result.UploadId);

                    var fileUploadRequestParts = new RestRequest(apiParts);

                    fileUploadRequestParts.AddHeader("X-Ingr-OnBehalfOf", pstrUserName);

                    fileUploadRequestParts.AddFileBytes(fileName, FileHelper.GetFileContent(fileParts[i]), fileName);

                    var responseParts = _restClient.restClient.Post(fileUploadRequestParts);

                    if (!responseParts.IsSuccessful)
                    {
                        string lstrErrorMessage = $"File Upload Request Not Successful for Part No {i}. URL {apiParts} Response: {responseParts.Content} ResponseErrorMessage {responseParts.ErrorMessage} ResponseErrorException {responseParts.ErrorException}";
                        Log.Error(lstrErrorMessage);
                        throw new Exception(lstrErrorMessage);
                    }
                }

                //Commmit the File Parts on the Server
                string apiCommit = "/FileMgmt/UploadFile?type=commit&UploadId=" + part1Result.UploadId;

                var reqCommit = new RestRequest(apiCommit);

                reqCommit.AddHeader("X-Ingr-OnBehalfOf", pstrUserName);

                var commitResponse = _restClient.restClient.Post(reqCommit);

                if (!commitResponse.IsSuccessful)
                {
                    string lstrErrorMessage = $"File Commit Request Not Successful. URL {apiCommit} Response: {commitResponse.Content} ResponseErrorMessage {commitResponse.ErrorMessage} ResponseErrorException {commitResponse.ErrorException}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                return JsonConvert.DeserializeObject<FileUploadResponse>(commitResponse.Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<string> SplitLargeFileintoChunks(string filePath, int maxFileSize)
        {
            try
            {
                List<string> files = new List<string>();

                // var tempForSplitedFiles = Path.Combine(_config.FileServiceFolder, "SplitedFilesForUploadTemp");

                var tempForSplitedFiles = Path.Combine(System.IO.Path.GetTempPath(), "SplitedFilesForUploadTemp");

                FileHelper.CreateFolderIfMissing(tempForSplitedFiles);

                var fileName = Path.GetFileName(filePath);

                int BufferChunckSize = maxFileSize * 1024 * 1024;

                const int READBUFFER_SIZE = 1024;

                byte[] FSBuffer = new byte[READBUFFER_SIZE];

                using (FileStream FS = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int TotalFileParts = 0;
                    if (FS.Length < BufferChunckSize)
                    {
                        files.Add(filePath);
                        return files;
                    }
                    else
                    {
                        float PreciseFileParts = ((float)FS.Length / (float)BufferChunckSize);
                        TotalFileParts = (int)Math.Ceiling(PreciseFileParts);
                    }

                    int FilePartCount = 0;
                    while (FS.Position < FS.Length)
                    {
                        string filePartName = String.Format("{0}.part_{1}.{2}", fileName, (FilePartCount + 1).ToString(), TotalFileParts.ToString());
                        string filePartPath = Path.Combine(tempForSplitedFiles, filePartName);
                        files.Add(filePartPath);

                        using (FileStream FilePart = new FileStream(filePartPath, FileMode.Create))
                        {
                            int bytesRemaining = BufferChunckSize;
                            int bytesRead = 0;
                            while (bytesRemaining > 0 && (bytesRead = FS.Read(FSBuffer, 0, Math.Min(bytesRemaining, READBUFFER_SIZE))) > 0)
                            {
                                FilePart.Write(FSBuffer, 0, bytesRead);
                                bytesRemaining -= bytesRead;
                            }
                        }
                        FilePartCount++;
                    }

                }

                return files;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UploadResponse CheckUploadStatus(string uid, string pstrUserName)
        {
            try
            {
                string apiUrl = String.Format("/FileMgmt/Upload('{0}')", uid);

                var uploadRequest = new RestRequest(apiUrl);

                uploadRequest.AddHeader("X-Ingr-OnBehalfOf", pstrUserName);

                var uploadResponse = _restClient.restClient.Get(uploadRequest);

                if (!uploadResponse.IsSuccessful)
                {
                    string lstrErrorMessage = $"CheckUploadStatus: Request Not Successful. URL {apiUrl} Response: {uploadResponse.Content} ResponseErrorMessage {uploadResponse.ErrorMessage} ResponseErrorException {uploadResponse.ErrorException}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                return JsonConvert.DeserializeObject<UploadResponse>(uploadResponse.Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool MakeUploadAvailable(string uploadId, string pstrUser)
        {
            try
            {
                var makeUploadAvailableApiUrl = $"{_restClient.restClient.BaseUrl}/FileMgmt/MakeUploadAvailable";

                var makeUploadAvailableReq = new RestRequest(makeUploadAvailableApiUrl);

                var makeUploadAvailableBody = new MakeFileAvailableRequest()
                {
                    Filename = uploadId
                };
                makeUploadAvailableReq.AddJsonBody(makeUploadAvailableBody);

                var resp = _restClient.restClient.Post(makeUploadAvailableReq);

                if (!resp.IsSuccessful)
                {
                    string lstrErrorMessage = $"MakeUploadAvailable: Request Not Successful. URL {makeUploadAvailableApiUrl} Response: {resp.Content} ResponseErrorMessage {resp.ErrorMessage} ResponseErrorException {resp.ErrorException}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                return resp.IsSuccessful;
            }
            catch
            {
                throw;
            }

        }

        public bool AttachFile(string uploadId, string fileName, string targetObjectOBID, string configUID)
        {
            try
            {
                var apiUrl = String.Format($"{_restClient.restClient.BaseUrl}/FileMgmt/Upload('{uploadId}')/Intergraph.SPF.Server.API.Model.Attach", uploadId);

                var attachRequest = new RestRequest(apiUrl);
                if (configUID != null)
                {
                    attachRequest.AddHeader("spfconfiguid", configUID);
                }

                var attachBody = new AttachFileRequest()
                {
                    ClientFilePath = fileName,
                    TargetObjectOBID = targetObjectOBID,
                    DeleteScannedFile = false
                };

                attachRequest.AddJsonBody(attachBody);

                var attachResp = _restClient.restClient.Post(attachRequest);

                if (!attachResp.IsSuccessful)
                {
                    string lstrErrorMessage = $"File {fileName} Not Attached to Target Object OBID {targetObjectOBID} URL {apiUrl} RequestBODY {JsonConvert.SerializeObject(attachBody)} Response: {attachResp.Content} ErrorException : {attachResp.ErrorException} ErrorMessage: {attachResp.ErrorMessage}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                return attachResp.IsSuccessful;
            }
            catch
            {
                throw;
            }
        }
        public bool MoveUploadedFileToDirectory(string uploadId, string fileName, string folderID, string pstrUserName)
        {
            try
            {
                var apiUrl = String.Format("/FileMgmt/Upload('{0}')/Intergraph.SPF.Server.API.Model.Move", uploadId);

                var moveFileBody = new MoveFileRequest()
                {
                    CleanupOnly = false,
                    ClientFileName = fileName,
                    DestinationDirectory = folderID
                };

                var moveFileRequest = new RestRequest(apiUrl);

                moveFileRequest.AddHeader("X-Ingr-OnBehalfOf", pstrUserName);

                moveFileRequest.AddJsonBody(moveFileBody);

                var moveFileResp = _restClient.restClient.Post(moveFileRequest);

                if (!moveFileResp.IsSuccessful)
                {
                    string lstrErrorMessage = $"MoveUploadedFileToDirectory: Request Not Successful. URL {apiUrl} Response: {moveFileResp.Content} ResponseErrorMessage {moveFileResp.ErrorMessage} ResponseErrorException {moveFileResp.ErrorException}";
                    Log.Error(lstrErrorMessage);
                    throw new Exception(lstrErrorMessage);
                }

                return moveFileResp.IsSuccessful;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public string CreateMarkupObj(string pstrURL, string MarkupFileReqBody, string pstrConfigUID, string pstrUser, string pstrAcceptParameter = "")
        {
            Log.Verbose("Begin: Create Markup Object API");
            string lstrReponse = string.Empty;

            try
            {
                var request = new RestRequest(pstrURL, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(pstrAcceptParameter))
                    request.AddHeader("Accept", "application/vnd.intergraph.data+json");
                else
                    request.AddHeader("Accept", "application/json,application/vnd.intergraph.columnset.CS_MarkupLayerManager+json");

                request.AddHeader("SPFConfigUID", pstrConfigUID);
                request.AddHeader("X-Ingr-OnBehalfOf", pstrUser);
                request.AddParameter("application/json", MarkupFileReqBody, ParameterType.RequestBody);
                IRestResponse response = response = _restClient.restClient.Post(request);
				if (!response.IsSuccessful)
				{
					throw new Exception();
				}
				lstrReponse = response.Content;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreateMarkUpObject");
            }
            Log.Verbose("End: CreateMarkUpObject API");
            return lstrReponse;
        }

        public string SaveMarkupObj(string pstrURL, string MarkupFileReqBody, string pstrConfigUID, string pstrUser)
        {
            Log.Verbose("Begin: Create Markup Object API");
            string lstrReponse = string.Empty;

            try
            {
                var request = new RestRequest(pstrURL, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json, text/plain, */*");
                request.AddHeader("SPFConfigUID", pstrConfigUID);
                request.AddHeader("X-Ingr-OnBehalfOf", pstrUser);
                request.AddParameter("application/json", MarkupFileReqBody, ParameterType.RequestBody);
                IRestResponse response = response = _restClient.restClient.Post(request);
                lstrReponse = response.Content;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreateMarkUpObject");
            }
            Log.Verbose("End: CreateMarkUpObject API");
            return lstrReponse;
        }

    }
}
