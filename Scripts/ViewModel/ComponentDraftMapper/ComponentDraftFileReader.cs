using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
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
                return JsonSerializer.Deserialize<ComponentDraft>(fileContent);
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
