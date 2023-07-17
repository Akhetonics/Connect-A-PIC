using ConnectAPIC.Scenes.Component;

namespace ConnectAPIC.Scenes.Compiler
{
    public class NazcaExporter : IExporter
    {
        private readonly string path;
        public NazcaExporter(string path)
        {
            this.path = path;
        }
        public string Export(Grid grid)
        {
            // TODO:
            return "";
        }
    }
}