namespace Components.ComponentDraftMapper
{
    public interface IDataAccesser : IResourcePathChecker
    {
        public string ReadAsText(string FilePath);
        bool Write(string filePath, string componentJson);
    }
}