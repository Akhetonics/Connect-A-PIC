
// To avoid naming collision with Godot. Sadly, Photonic Components have the same name.
public class ComponentItem
{  
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }

    public ComponentItem(ComponentItemPrototype prototype)
    {
        this.SizeX = prototype.SizeX;
        this.SizeY = prototype.SizeY;
    }
}