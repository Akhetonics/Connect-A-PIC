namespace ConnectAPIC.Scenes.Component
{
    public class ConnectionPort
    {
        public string Name { get; private set; } // the nazca name like b0, a0, a1

        public ConnectionPort(string name)
        {
            // Name should be unique to the component
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}