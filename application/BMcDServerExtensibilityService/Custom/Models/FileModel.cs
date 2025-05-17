using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Custom.Models
{
    public  class FileModel
    {
        public partial class FileRequest
        {
            public string FileName { get; set; }
            public byte[] FileContent { get; set; }
        }
        public partial class FileUploadResponse
        {
            public string UploadId { get; set; }
            public string Status { get; set; }
            public string Expires { get; set; }
        }
        public partial class MakeFileAvailableRequest
        {
            public string Filename { get; set; }
        }
        public partial class AttachFileRequest
        {
            public string ClientFilePath { get; set; }
            public string TargetObjectOBID { get; set; }
            public bool DeleteScannedFile { get; set; }
        }
        public partial class MoveFileRequest
        {
            public string ClientFileName { get; set; }
            public string DestinationDirectory { get; set; }
            public bool CleanupOnly { get; set; }
        }
        public partial class UploadResponse
        {
            public string FileStatus { get; set; }
            public string Hash { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
        }
    }
}
