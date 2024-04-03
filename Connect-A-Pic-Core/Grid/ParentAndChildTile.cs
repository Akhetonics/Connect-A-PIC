using CAP_Core.Tiles;

namespace CAP_Core.Grid
{
    public record struct ParentAndChildTile(Tile ParentPart, Tile Child)
    {
        public static implicit operator (Tile, Tile)(ParentAndChildTile value)
        {
            return (value.ParentPart, value.Child);
        }

        public static implicit operator ParentAndChildTile((Tile, Tile) ParentAndChild)
        {
            return new ParentAndChildTile(ParentAndChild.Item1, ParentAndChild.Item2);
        }
    }
}
