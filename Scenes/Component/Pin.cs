namespace ConnectAPIC.Scenes.Component
{
    public class Pin
    {
        public string Name { get; private set; } // the nazca name like b0, a0, a1
        public MatterType MatterType;
        public Pin Connection;
        public Pin(string name, MatterType matterType)
        {
            // Name should be unique to the component
            this.MatterType = matterType;
            Name = name;
        }
        public void Connect(Pin otherPin)
        {
            this.Connection = otherPin;
            otherPin.Connection = this;
        }
        public void Disconnect(){
            if (this.Connection == null) return;
            var oldConnection = this.Connection;
            this.Connection = null;
            if (oldConnection.Connection != null)
            {
                oldConnection.Disconnect();
            }
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}