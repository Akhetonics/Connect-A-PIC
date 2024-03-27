using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public interface IComponentRelationshipManager
    {
        public List<ParentAndChildTile> GetConnectedNeighborsOfComponent(Component component);
    }
}
