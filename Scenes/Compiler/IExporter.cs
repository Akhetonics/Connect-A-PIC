using ConnectAPIC.Scenes.Component;

namespace ConnectAPIC.Scenes.Compiler
{
    interface IExporter
    {
        string Export(Tile[,] Tiles);
    }
}