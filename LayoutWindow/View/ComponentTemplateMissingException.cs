using System;

namespace ConnectAPIC.Scenes.Component
{
    public class ComponentTemplateMissingException : Exception
    {
        public ComponentTemplateMissingException(Type T) : base(T.Name + " is not attached to the ComponentFactory. The dev should use the Godot Editor to add it")
        {
        }
    }
}