using CAP_DataAccess.Helpers;
using Components.ComponentDraftMapper.DTOs;
using Components.ComponentDraftMapper;
using CAP_Contracts;

namespace CAP_DataAccess.Components.ComponentDraftMapper
{
    public class ComponentJSONFinder : FileFinder
    {
        public ComponentJSONFinder(IDirectoryAccess directoryAccess, IDataAccessor dataAccessor) : base(directoryAccess)
        {
            DataAccessor = dataAccessor;
            Validator = new(dataAccessor);
        }
        public IDataAccessor DataAccessor { get; }
        public ComponentDraftValidator Validator { get; }
        public List<(ComponentDraft? draft,string error)> ReadComponentJSONDrafts(string componentFolderPath)
        {
            ComponentDraftFileReader reader = new(DataAccessor);
            var draftsAndErrors = FindRecursively(componentFolderPath, "json")
                .Select(file => reader.TryReadJson(file))
                .ToList();

            // validate the list and add errors if elements are invalid
            for (int i = 0; i < draftsAndErrors.Count; i++)
            {
                if (draftsAndErrors[i].draft == null) continue;
                (bool isValid, string errorMsg) = Validator.Validate(draftsAndErrors[i].draft);
                if (isValid) continue;
                draftsAndErrors[i] = (draftsAndErrors[i].draft, errorMsg);
            }

            return draftsAndErrors;
        }
    }
}
