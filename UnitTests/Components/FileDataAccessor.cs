using Components.ComponentDraftMapper;

namespace UnitTests
{
    public partial class ComponentDraftFileReaderTests
    {
        public class FileDataAccessor : IDataAccessor
        {
            public bool DoesResourceExist(string resourcePath) => File.Exists(resourcePath);
            public string ReadAsText(string filePath) => File.ReadAllText(filePath);
            public async Task<bool> Write(string filePath, string componentJson) {
                await File.WriteAllTextAsync(filePath , componentJson);
                return true;
            }
        }
    }
}
