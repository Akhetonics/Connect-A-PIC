﻿using Components.ComponentDraftMapper;
using Godot;

namespace ConnectAPIC.Scripts.View.ComponentFactory
{
    public class DataAccessorGodot : IDataAccessor
    {
        public bool DoesResourceExist(string godotFilePath)
        {
            if (!godotFilePath.StartsWith("res://Scenes/Components"))
            {
                return false;
            }
            return true;// the path is within a pck file and we cannot load PCK files without importing them.. So `FileAccess.FileExists(godotFilePath);` won't work.
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
