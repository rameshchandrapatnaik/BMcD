using System.IO;

namespace RILExtensibilityService.Infra.Services
{
    public class FileHelper
    {
        public static byte[] GetFileContent(string path)
        {
            byte[] fileContent;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fileContent = new byte[fs.Length];
                fs.Read(fileContent, 0, (int)fs.Length);
            }

            return fileContent;
        }

        public static void CreateFolderIfMissing(string path)
        {
            bool exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
