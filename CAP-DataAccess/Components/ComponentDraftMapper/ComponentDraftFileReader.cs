using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CAP_Contracts;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;

namespace CAP_DataAccess.Components.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static readonly int CurrentFileVersion = 1;

        public ComponentDraftFileReader(IDataAccessor dataAccessor)
        {
            DataAccessor = dataAccessor;
        }

        public IDataAccessor DataAccessor { get; }

        /// <summary>
        /// error is null if everything goes right, otherwise draft is null and error is the error description.
        /// it does not throw any exception directly
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public (ComponentDraft? draft, string? error) TryReadJson(string path)
        {
            if (DataAccessor.DoesResourceExist(path))
            {
                var fileContent = DataAccessor.ReadAsText(path);
                if (fileContent != null)
                {
                    try
                    {   
                        return (JsonSerializer.Deserialize<ComponentDraft>(fileContent) ?? new ComponentDraft(), null);
                    }
                    catch (Exception ex)
                    {
                        var err = "Error Deserializing Json-file: " + path + "  ex: " + ex.Message + " " + ex.StackTrace;
                        return (null , err);
                    }
                }
                else
                {
                    return (null , "could not open file to read, it might be blocked somehow: " + path);
                }
            }
            return (null, "File does not exist: " + path);
        }

        public async Task<(bool isSuccess,string error)> Write(string filePath, ComponentDraft componentDraft)
        {
            var componentJson = JsonSerializer.Serialize(componentDraft);
            bool success = await DataAccessor.Write(filePath, componentJson);
            if (!success)
            {
                return (success, "Failed to open file for writing: " + filePath);
            }
            return (success, "");
        }
    }
}
