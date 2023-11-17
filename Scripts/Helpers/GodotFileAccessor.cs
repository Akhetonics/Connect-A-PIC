using Godot;

namespace Components.ComponentDraftMapper
{
    public class GodotFileAccessor : IDataAccesser
    {
        public bool DoesResourceExist(string godotFilePath)
        {
            return FileAccess.FileExists(godotFilePath);
        }

        public string ReadAsText(string godotFilePath)
        {
            FileAccess file = FileAccess.Open(godotFilePath, FileAccess.ModeFlags.Read);
            if (file != null)
            {
                string fileContent = file.GetAsText();
                file.Close();
                return fileContent;
            }
            else
            {
                throw new System.IO.FileNotFoundException(godotFilePath);
            }
        }

        public bool Write(string godotFilePath, string componentJson)
        {
            var file = FileAccess.Open(godotFilePath, FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreString(componentJson);
                file.Close();
                return true;
            }
            return false;
        }
    }
}
