using CAP_Core.Tiles;
using static CAP_Core.Grid.GridManager;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class TileManager : ITileManager
    {
        public event OnGridCreatedHandler OnGridCreated;
        public Tile[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int MinHeight = 10;

        public TileManager(int width, int height)
        {
            if (height < MinHeight) height = MinHeight;
            Width = width;
            Height = height;
            Tiles = new Tile[Width, Height];
            GenerateAllTiles(Tiles);
        }
        private void GenerateAllTiles(Tile[,] tiles)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tiles[x, y] = new Tile(x, y);
                }
            }
            OnGridCreated?.Invoke(tiles);
        }
        public bool IsCoordinatesInGrid(int x, int y, int width = 1, int height = 1)
        {
            return x >= 0 && y >= 0 && x + width <= Width && y + height <= Height;
        }

        public List<Component> GetAllComponents()
        {
            List<Component> components = new();
            foreach (Tile tile in Tiles)
            {
                if (tile.Component == null) continue;
                components.Add(tile.Component);
            }
            return components.Distinct().ToList();
        }
    }
}
