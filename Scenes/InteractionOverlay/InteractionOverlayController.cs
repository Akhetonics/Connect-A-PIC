using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.InteractionOverlay
{
    internal class InteractionOverlayController
    {
        public List<OverlayElement> ObservedElements;
        public List<OverlayElement> ActiveElements;

        public int CurrentMaxZIndex {  set; get; }
        public bool ScrollingAllowed { set; get; }
        public bool ClikcingAllowed { set; get; }

        public Hashtable ScrollingPremissionByZIndex { set; get; } = new();
        public Hashtable ClickingPremissionByZIndex { set; get; } = new();

        //TODO: add connect/disconnect functionallity
        //TODO: figure oute hash tbles stuff
        //TODO: Determine activity change (maybe emit from elements if their visibility changes and then remove them from active elements?)
    }
}
