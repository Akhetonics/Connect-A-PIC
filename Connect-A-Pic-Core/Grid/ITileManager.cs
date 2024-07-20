using CAP_Core.Tiles;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public interface ITileManager
    {
        Tile[,] Tiles { get; }
        int Width { get; }
        int Height { get; }
        bool IsCoordinatesInGrid(int x, int y, int width = 1, int height = 1);

        List<Component> GetAllComponents();
    }
}
