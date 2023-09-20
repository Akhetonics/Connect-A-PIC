using System;
using System.Runtime.Serialization;

namespace CAP_Core.Component.ComponentHelpers
{
    [Serializable]
    public class ComponentCannotBePlacedException : Exception
    {
        public string ComponentTypeName { get; set; }
        public string BlockingComponentInfo { get; set; }
        public ComponentCannotBePlacedException(ComponentBase component, ComponentBase blockingComponent)
            : base(blockingComponent.ToString()
                  + "\n## Unable to Place this component ##: \n"
                  + component.ToString())
        {
            ComponentTypeName = component.GetType().FullName;
            BlockingComponentInfo = blockingComponent.ToString();
        }
        protected ComponentCannotBePlacedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ComponentTypeName = info.GetString(nameof(ComponentTypeName));
            BlockingComponentInfo = info.GetString(nameof(BlockingComponentInfo));
        }

        public new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ComponentTypeName), ComponentTypeName);
            info.AddValue(nameof(BlockingComponentInfo), BlockingComponentInfo);
        }
    }
}