using CAP_Core.Component.ComponentDraftMapper.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CAP_Core.Component.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static ComponentDraft? Read(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<ComponentDraft>(fileContent);
            } else
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
