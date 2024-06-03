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

        public static bool ScrollingAllowed { set; get; }
        public static bool ClickingAllowed { set; get; }

        private static bool IsMaxIndexSet { set; get; } = false;
        private static int CurrentMaxZIndex { set; get; } = int.MinValue;

        private static List<OverlayElement> ObservedElements { set; get; } = new();
        private static Dictionary<int, int> ScrollingPremissionByZIndex { set; get; } = new();
        private static Dictionary<int, int> ClickingPremissionByZIndex { set; get; } = new();


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
            AddPremissionValuesOfElement(element);

            if (CurrentMaxZIndex < element.OverlayZIndex)
            {
                CurrentMaxZIndex = element.OverlayZIndex;
                IsMaxIndexSet = true;
            }
            GD.Print("Entered!");
            ApplyMouseBehaviourByMaxIndex();
        }

        public static void MouseExitedElement(object sender, OverlayElement element)
        {
            RemovePremissionValuesOfElement(element);
            SetNewMaxIndex();
            GD.Print("Exited!");
            ApplyMouseBehaviourByMaxIndex();
        }

        private static void SetNewMaxIndex()
        {
            if (ScrollingPremissionByZIndex.Count == 0) // scroling and clicking premission dictionaries are equal in size
            {
                IsMaxIndexSet = false;
                CurrentMaxZIndex = int.MinValue;
                return;
            }

            var zIndecies = ScrollingPremissionByZIndex.Keys;

            CurrentMaxZIndex = zIndecies.Max();
            IsMaxIndexSet = true;
        }

        private static void ApplyMouseBehaviourByMaxIndex()
        {
            if (!IsMaxIndexSet)
            {
                ScrollingAllowed = DefaultScrollValue;
                ClickingAllowed = DefaultClickValue;
                return;
            }

            var scrollVal = ScrollingPremissionByZIndex[CurrentMaxZIndex];
            var clickVal = ClickingPremissionByZIndex[CurrentMaxZIndex];

            if (scrollVal > 0) ScrollingAllowed = true;
            else if (scrollVal < 0) ScrollingAllowed = false;
            else ScrollingAllowed = DefaultScrollValue;

            if (clickVal > 0) ClickingAllowed = true;
            else if (clickVal < 0) ClickingAllowed = false;
            else ClickingAllowed = DefaultClickValue;
        }

        private static void AddPremissionValuesOfElement(OverlayElement element){
            var zIndex = element.OverlayZIndex;

            if (!ScrollingPremissionByZIndex.ContainsKey(zIndex))
                ScrollingPremissionByZIndex[zIndex] = 0;

            ScrollingPremissionByZIndex[zIndex] += (element.Scrolling ? 1 : -1);

            if (!ClickingPremissionByZIndex.ContainsKey(zIndex))
                ClickingPremissionByZIndex[zIndex] = 0;

            ClickingPremissionByZIndex[zIndex] += (element.Clicking ? 1 : -1);
        }

        private static void RemovePremissionValuesOfElement(OverlayElement element)
        {
            var zIndex = element.OverlayZIndex;

            if (ScrollingPremissionByZIndex.ContainsKey(zIndex)){
                ScrollingPremissionByZIndex[zIndex] -= (element.Scrolling ? 1 : -1);
                if (ScrollingPremissionByZIndex[zIndex] == 0)
                    ScrollingPremissionByZIndex.Remove(zIndex);
            }

            if (ClickingPremissionByZIndex.ContainsKey(zIndex)){
                ClickingPremissionByZIndex[zIndex] -= (element.Clicking ? 1 : -1);
                if (ClickingPremissionByZIndex[zIndex] == 0)
                    ClickingPremissionByZIndex.Remove(zIndex);
            }
        }
    }
}
