using CAP_Contracts;

namespace CAP_Core.Helpers
{
    public class FileDataAccessor : IDataAccessor
    {
        public bool DoesResourceExist(string resourcePath) => File.Exists(resourcePath);
        public string ReadAsText(string filePath) => File.ReadAllText(filePath);
        public async Task<bool> Write(string filePath, string componentJson)
        {
            if (IsValidFilePath(filePath) == false)
            {
                return false;
            }

            await File.WriteAllTextAsync(filePath, componentJson);
            return true;
        }
        private static bool IsValidFilePath(string filePath)
        {
            try
            {
                Path.GetFullPath(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
