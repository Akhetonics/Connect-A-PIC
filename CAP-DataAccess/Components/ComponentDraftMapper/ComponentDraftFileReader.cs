using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Components.ComponentDraftMapper.DTOs;

namespace Components.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static readonly int CurrentFileVersion = 1;

        public ComponentDraftFileReader(IDataAccessor dataAccessor)
        {
            DataAccessor = dataAccessor;
        }

        public IDataAccessor DataAccessor { get; }

        public (ComponentDraft? draft, string error) TryRead(string godotFilePath)
        {
            if (DataAccessor.DoesResourceExist(godotFilePath))
            {
                var fileContent = DataAccessor.ReadAsText(godotFilePath);
                if (fileContent != null)
                {
                    try
                    {   
                        return (JsonSerializer.Deserialize<ComponentDraft>(fileContent) ?? new ComponentDraft(),"");
                    }
                    catch (Exception ex)
                    {
                        var err = "Error Deserializing Json-file: " + godotFilePath + "  ex: " + ex.Message + " " + ex.StackTrace;
                        return (null , err);
                    }
                }
                else
                {
                    return (null , "could not open file to read, it might be blocked somehow: " + godotFilePath);
                }
            }
            return (null, "File does not exist: " + godotFilePath);
        }

        public (bool isSuccess,string error) Write(string filePath, ComponentDraft componentDraft)
        {
            var componentJson = JsonSerializer.Serialize(componentDraft);
            bool success = DataAccessor.Write(filePath, componentJson);
            if (!success)
            {
                return (success, "Failed to open file for writing: " + filePath);
            }
            return (success, "");
        }
    }
}
