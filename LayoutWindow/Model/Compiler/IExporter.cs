using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.Scenes.Component;
using Model;
using System.Collections.Generic;
using Tiles;

namespace ConnectAPIC.Scenes.Compiler
{
    interface IExporter
    {
        string Export(Grid grid);
    }
}