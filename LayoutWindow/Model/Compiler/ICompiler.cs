using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.Scenes.Component;
using Model;
using System.Collections.Generic;
using Tiles;

namespace ConnectAPIC.Scenes.Compiler
{
    public interface ICompiler
    {
        public string Compile(Grid grid);
    }
}