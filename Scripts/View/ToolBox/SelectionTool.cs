using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class SelectionTool : ToolBase, IToolPreviewing
    {
        public TemplateTileView CreateIcon()
        {
            TemplateTileView preview = new();
            var toolTilePixelSize = GameManager.TilePixelSize - GameManager.TileBorderLeftDown;
            preview.Texture = ResourceLoader.Load<Texture2D>("res://GFX/SelectTool.png");
            preview.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
            preview.SetMeta(ToolIDMetaName, ID.ToString());
            return preview;
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (IsActive)
            {
                
            }
        }

    }
}
