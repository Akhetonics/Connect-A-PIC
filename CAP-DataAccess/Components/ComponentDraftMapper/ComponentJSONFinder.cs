using CAP_DataAccess.Helpers;
using Components.ComponentDraftMapper.DTOs;
using Components.ComponentDraftMapper;

namespace CAP_DataAccess.Components.ComponentDraftMapper
{
    public class ComponentJSONFinder : FileFinder
    {
        public ComponentJSONFinder(IDirectoryAccess directoryAccess, IDataAccessor dataAccessor) : base(directoryAccess)
        {
            
            DataAccessor = dataAccessor;
        }
        public IDataAccessor DataAccessor { get; }
        public List<(ComponentDraft? draft,string error)> ReadComponentJSONDrafts(string componentFolderPath)
        {
            ComponentDraftFileReader reader = new(DataAccessor);
            return FindRecursively(componentFolderPath, "json")
                .Select(file => reader.TryRead(file))
                .ToList();
        }
    }
}
