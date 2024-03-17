using Godot;
using System;
using System.Collections.Generic;

public partial class InfoSection : ISection
{
    public InfoSection Initialize(String title, String value)
    {
        Title = title;
        Value = value;
        return this;
    }

}
