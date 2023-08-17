﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.Tiles
{
    public enum RectSide // has to have the same order as TurnableDegrees so that we can use the enum value as an index
    {
        Right,
        Down,
        Left,
        Up,
    }
}