using System;
using System.Text.Json;
using Components.ComponentDraftMapper.DTOs;

namespace Components.ComponentDraftMapper
{
    public class ComponentDraftFileReader
    {
        public static readonly int CurrentFileVersion = 1;

        public ComponentDraftFileReader(IDataAccesser dataAccessor, ILogger logger)
        {
            DataAccessor = dataAccessor;
            Logger = logger;
        }

        public IDataAccesser DataAccessor { get; }
        public ILogger Logger { get; }

        public ComponentDraft TryRead(string godotFilePath)
        {
            if (DataAccessor.DoesResourceExist(godotFilePath))
            {
                var fileContent = DataAccessor.ReadAsText(godotFilePath);
                if (fileContent != null)
                {

                    try
                    {
                        return JsonSerializer.Deserialize<ComponentDraft>(fileContent) ?? new ComponentDraft();
                    }
                    catch (Exception ex)
                    {
                        Logger?.PrintErr("Error Deserializing Json-file: " + godotFilePath + "  ex: " + ex.Message + " " + ex.StackTrace);
                        throw;
                    }
                }
                else
                {
                    throw new FileNotFoundException("Could not open file: " + godotFilePath);
                }
            }
            else
            {
                throw new FileNotFoundException("File not found: " + godotFilePath);
            }
        }

        public void Write(string filePath, ComponentDraft componentDraft)
        {
            var componentJson = JsonSerializer.Serialize(componentDraft);
            bool success = DataAccessor.Write(filePath, componentJson);
            
            if (!success)
            {
                Logger.PrintErr("Failed to open file for writing: " + filePath);
                throw new IOException("Could not open file for writing: " + filePath);
            }
        }
    }
}
