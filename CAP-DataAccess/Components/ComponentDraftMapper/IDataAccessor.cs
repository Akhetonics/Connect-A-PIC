namespace Components.ComponentDraftMapper
{
    public interface IDataAccessor : IResourcePathChecker
    {
        public string ReadAsText(string FilePath);
        bool Write(string filePath, string componentJson);
    }
}