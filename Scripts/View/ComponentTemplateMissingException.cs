using System;
using System.Runtime.Serialization;

namespace ConnectAPIC.LayoutWindow.View
{
    [Serializable]
    public class ComponentTemplateMissingException : Exception
    {
        public string ComponentTypeName { get; set; }
        public ComponentTemplateMissingException(string typeName) : base(typeName+ " is not attached to the ComponentFactory. The dev should use the Godot Editor to add it")
        {
            ComponentTypeName = typeName;
        }
        protected ComponentTemplateMissingException(SerializationInfo info, StreamingContext context) : base(info,context)
        {
            ComponentTypeName = info.GetString(nameof(ComponentTypeName));
        }

        public new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ComponentTypeName), ComponentTypeName);
        }
    }
}