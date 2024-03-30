using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.RightClickMenu.Sections {
    public partial class InfoSection : ISection {
        public void Initialize(String title, String value) {
            Title = title;
            Value = value;
        }

    }
}
