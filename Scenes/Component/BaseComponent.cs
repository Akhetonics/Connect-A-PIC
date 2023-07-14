
namespace ConnectAPIC.Scenes.Component
{
    public class Component
    {
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public Component(int TileWidth, int TileHeight)
        {
            this.TileWidth = TileWidth;
            this.TileHeight = TileHeight;
        }
    }
}