using ConnectAPIC.Scenes.Component;
using Tiles;

namespace ConnectAPIC.Scenes.Compiler
{
    interface IExporter
    {
        string Export(Tile[,] Tiles);
    }
}