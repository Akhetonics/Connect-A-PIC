using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.InteractionOverlay
{
    public partial class InteractionOverlayController : Node
    {
        /// <summary>
        /// Default value for scrolling (this value will be chosen if mouse cursor is outside of every element,
        ///     or if mouse behaviour is cancelled out by elements, like in case of 2 elements on the same z index
        ///     where one restricts scrolling and other allows it then default value will be chosen)
        /// </summary>
        public static bool DefaultScrollValue { set; get; } = true;

        /// <summary>
        /// Default value for clicking (this value will be chosen if mouse cursor is outside of every element,
        ///     or if mouse behaviour is cancelled out by elements, like in case of 2 elements on the same z index
        ///     where one restricts clicking and other allows it then default value will be chosen)
        /// </summary>
        public static bool DefaultClickValue { set; get; } = true;

        public static bool UseOnlyDefaults = false;

        public static bool ScrollingAllowed { get => IsMaxIndexSet && !UseOnlyDefaults ? ScrollingPermissionByZIndex[CurrentMaxZIndex] > 0 : DefaultScrollValue; }
        public static bool ClickingAllowed { get => IsMaxIndexSet && !UseOnlyDefaults ? ClickingPermissionByZIndex[CurrentMaxZIndex] > 0 : DefaultClickValue; }

        private static bool IsMaxIndexSet { set; get; } = false;
        private static int CurrentMaxZIndex { set; get; } = int.MinValue;

        private static List<OverlayElement> ObservedElements { set; get; } = new();
        private static Dictionary<int, int> ScrollingPermissionByZIndex { set; get; } = new();
        private static Dictionary<int, int> ClickingPermissionByZIndex { set; get; } = new();


        public static void Connect(OverlayElement element){
            if (ObservedElements.Contains(element)) return;
            ObservedElements.Add(element);
            element.AreaEntered += MouseEnteredElement;
            element.AreaExited += MouseExitedElement;
            if (element.MouseInsideAreas)
                MouseEnteredElement(element, element);

        }

        public static void Disconnect(OverlayElement element){
            ObservedElements.Remove(element);
            MouseExitedElement(element, element);
            element.AreaEntered -= MouseEnteredElement;
            element.AreaExited -= MouseExitedElement;
        }

        public static void MouseEnteredElement(object sender, OverlayElement element){
            AddPermissionValuesOfElement(element);

            if (CurrentMaxZIndex < element.OverlayZIndex)
            {
                CurrentMaxZIndex = element.OverlayZIndex;
                IsMaxIndexSet = true;
            }
            GD.Print("Entered!");
        }

        public static void MouseExitedElement(object sender, OverlayElement element)
        {
            RemovePermissionValuesOfElement(element);
            SetNewMaxIndex();
            GD.Print("Exited!");
        }

        private static void SetNewMaxIndex()
        {
            if (ScrollingPermissionByZIndex.Count == 0) // scrolling and clicking permission dictionaries are equal in size
            {
                IsMaxIndexSet = false;
                CurrentMaxZIndex = int.MinValue;
                return;
            }

            var zIndexes = ScrollingPermissionByZIndex.Keys;

            CurrentMaxZIndex = zIndexes.Max();
            IsMaxIndexSet = true;
        }


        private static void AddPermissionValuesOfElement(OverlayElement element){
            var zIndex = element.OverlayZIndex;

            if (!ScrollingPermissionByZIndex.ContainsKey(zIndex))
                ScrollingPermissionByZIndex[zIndex] = 0;

            ScrollingPermissionByZIndex[zIndex] += (element.Scrolling ? 1 : -1);

            if (!ClickingPermissionByZIndex.ContainsKey(zIndex))
                ClickingPermissionByZIndex[zIndex] = 0;

            ClickingPermissionByZIndex[zIndex] += (element.Clicking ? 1 : -1);
        }

        private static void RemovePermissionValuesOfElement(OverlayElement element)
        {
            var zIndex = element.OverlayZIndex;

            if (ScrollingPermissionByZIndex.ContainsKey(zIndex)){
                ScrollingPermissionByZIndex[zIndex] -= (element.Scrolling ? 1 : -1);
                if (ScrollingPermissionByZIndex[zIndex] == 0)
                    ScrollingPermissionByZIndex.Remove(zIndex);
            }

            if (ClickingPermissionByZIndex.ContainsKey(zIndex)){
                ClickingPermissionByZIndex[zIndex] -= (element.Clicking ? 1 : -1);
                if (ClickingPermissionByZIndex[zIndex] == 0)
                    ClickingPermissionByZIndex.Remove(zIndex);
            }
        }
    }
}
