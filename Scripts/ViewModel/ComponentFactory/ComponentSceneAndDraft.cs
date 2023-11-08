using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;

namespace ConnectAPIC.LayoutWindow.View
{
    public record struct ComponentSceneAndDraft(PackedScene Scene, ComponentDraft Draft)
    {
        public static implicit operator (PackedScene scene, ComponentDraft componentDraft)(ComponentSceneAndDraft value)
        {
            return (value.Scene, value.Draft);
        }

        public static implicit operator ComponentSceneAndDraft((PackedScene Scene, ComponentDraft Draft) value)
        {
            return new ComponentSceneAndDraft(value.Scene, value.Draft);
        }
    }
}
