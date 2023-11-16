using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System;
using System.Text.Json;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static readonly int CurrentFileVersion = 1;

        public static ComponentDraft TryRead(string filePath)
        {
            if (FileAccess.FileExists(filePath))
            {
                Godot.FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
                if (file != null)
                {
                    string fileContent = file.GetAsText();
                    file.Close();

                    try
                    {
                        return JsonSerializer.Deserialize<ComponentDraft>(fileContent);
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.PrintErr("Error Deserializing Json-file: " + filePath + "  ex: " + ex.Message + " " + ex.StackTrace);
                        return null;
                    }
                }
                else
                {
                    throw new System.IO.FileNotFoundException("Could not open file: " + filePath);
                }
            }
            else
            {
                throw new System.IO.FileNotFoundException("File not found: " + filePath);
            }
        }

        public static void Write(string filePath, ComponentDraft componentDraft)
        {
            var componentJson = JsonSerializer.Serialize(componentDraft);

            var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreString(componentJson);
                file.Close();
            }
            else
            {
                CustomLogger.PrintErr("Failed to open file for writing: " + filePath);
                throw new System.IO.IOException("Could not open file for writing: " + filePath);
            }
        }
    }
}
