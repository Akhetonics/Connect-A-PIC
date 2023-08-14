using ConnectAPIC.Scenes.Component;
using System;
using System.Runtime.Serialization;

namespace ConnectAPIC.LayoutWindow.Model.Component
{
    [Serializable]
    public class ComponentCannotBePlacedException : Exception
    {
        public string ComponentTypeName { get; set;}
        public ComponentCannotBePlacedException(ComponentBase component) : base(component.ToString())
        {
            ComponentTypeName = component.GetType().FullName;
        }
        protected ComponentCannotBePlacedException(SerializationInfo info, StreamingContext context) : base(info, context)
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