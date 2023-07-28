using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public class StraightWaveGuideView
    {
        [Export] Texture2D Texture;
        public void display()
        {
            Console.WriteLine("ComponentView.display()");
        }
    }
}
