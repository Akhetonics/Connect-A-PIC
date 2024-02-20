using CAP_Core.Tiles;

namespace CAP_Core.Components.ComponentHelpers
{
    public static class ComponentExtensions
    {
        public static Guid? PinIdRightIn(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Right)?.IDInFlow;
        }
        public static Guid? PinIdRightOut(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Right)?.IDOutFlow;
        }
        public static Guid? PinIdDownIn(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Down)?.IDInFlow;
        }
        public static Guid? PinIdDownOut(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Down)?.IDOutFlow;
        }
        public static Guid? PinIdLeftIn(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Left)?.IDInFlow;
        }
        public static Guid? PinIdLeftOut(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Left)?.IDOutFlow;
        }
        public static Guid? PinIdUpIn(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Up)?.IDInFlow;
        }
        public static Guid? PinIdUpOut(this Component component, int offsetX = 0, int offsetY = 0)
        {
            return component.Parts[offsetX, offsetY].GetPinAt(RectSide.Up)?.IDOutFlow;
        }
    }
}
