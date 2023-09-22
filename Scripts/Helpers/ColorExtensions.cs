using CAP_Core.ExternalPorts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class ColorExtensions
    {
        public static Godot.Color ToGodotColor(this System.Drawing.Color sysColor)
        {
            float r = sysColor.R / 255f;
            float g = sysColor.G / 255f;
            float b = sysColor.B / 255f;
            float a = sysColor.A / 255f;

            return new Godot.Color(r, g, b, a);
        }
        public static Godot.Color ToGodotColor(this LightColor lightColor)
        {
            var sysColor = lightColor.ToColor();
            float r = sysColor.R / 255f;
            float g = sysColor.G / 255f;
            float b = sysColor.B / 255f;
            float a = sysColor.A / 255f;

            return new Godot.Color(r, g, b, a);
        }
    }
}
