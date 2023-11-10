using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using System;
using System.IO;
using System.Text.Json;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static readonly int CurrentFileVersion = 1;

        public static ComponentDraft Read(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath);
                try
                {
                    return JsonSerializer.Deserialize<ComponentDraft>(fileContent);
                }catch (Exception ex)
                {
                    CustomLogger.PrintErr("file: " + filePath + "  ex: " + ex.Message + " " + ex.StackTrace);
                    throw;
                }
                
            }
            else
            {
                throw new FileNotFoundException(filePath);
            }
        }

        public static void Write(string filePath, ComponentDraft componentDraft)
        {
            var componentJson = JsonSerializer.Serialize(componentDraft);
            File.WriteAllTextAsync(filePath, componentJson);
        }
    }
}
