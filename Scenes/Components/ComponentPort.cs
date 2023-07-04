public class ComponentPort
{
    public string Name { get; private set; }

    public ComponentPort(string name)
    {
        // Name should be unique
        this.Name = name;
    }

    public override string ToString()
    {
        return this.Name;
    }
}