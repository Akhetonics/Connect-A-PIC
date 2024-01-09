namespace CAP_Contracts
{
    public interface IDataAccessor : IResourcePathChecker
    {
        public string ReadAsText(string FilePath);
        Task<bool> Write(string filePath, string text);
        
    }
}