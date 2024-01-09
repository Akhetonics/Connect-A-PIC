using CAP_Core.Grid;

namespace CAP_Core.CodeExporter
{
    public interface IExporter
    {
        public string Export(GridManager grid);
    }
}