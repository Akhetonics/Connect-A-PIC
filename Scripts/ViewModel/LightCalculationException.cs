using ConnectAPIC.LayoutWindow.View;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    [Serializable]
    public class LightCalculationException : Exception
    {
        public List<Exception> exceptions { get; private set; }

        public LightCalculationException()
        {
        }

        public LightCalculationException(List<Exception> exceptions, List<AnimationSlot> animationSlots, List<LightAtPin> lightAtPins)
        {
            this.exceptions = exceptions;
        }

        public LightCalculationException(string message) : base(message)
        {
        }

        public LightCalculationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LightCalculationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}