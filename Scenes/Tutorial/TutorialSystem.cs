using Godot;
using System;

public partial class TutorialSystem : Control
{
    [Export] Control TutorialPopup { get; set; }
   
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Control ExclusionZoneContainer { get; set; }
    [Export] TextureRect ExclusionCircle { get; set; }
    [Export] TextureRect ExclusionSquare { get; set; }

    [Export] Node2D PortContainer { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Control ToolBoxContainer { get; set; }


    private RichTextLabel Title;
    private RichTextLabel Body;

    private Control YesNoConfiguration;
    private Control QuitSkipNextConfig;
    private Control SkipContainer;
    private Control NextContainer;


    private int portContainerOffset = 124;
    private int portsWidth = 120;
    private int portHeight = 62;

    /// <summary>
    /// Represents height and width of a square tile in grid
    /// </summary>
    private int tileSize = 62;

    /// <summary>
    /// Represents global position of top left corner of the grid
    /// </summary>
    private Vector2 gridGlobalPosition;

    /// <summary>
    /// Represents the view port position of menu buttons (default left top corner)
    /// </summary>
    private Vector2 menuButtonPosition = Vector2.Zero;

    int i = 0;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            // Check if the space key is pressed
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                //TODO: debugging code remove afterwards
                if (i == 0)
                {
                    ClearExclusionZones();
                    HighlightControlNodeWithCustomSize(PortContainer, customXOffset: -portsWidth, customYOffset: portContainerOffset, customXSize: portsWidth, customYSize: portHeight * 8);
                    i++;
                    SetTutorialPopupCenter();
                    SetYesNoConfiguration();
                }
                else if (i == 1)
                {
                    ClearExclusionZones();
                    HighlightControlNode(MenuBar, allMargins:3f);
                    i++;
                    SetTutorialPopupTopRight();
                    SetQuitNextConfiguration();
                }
                else
                {
                    ClearExclusionZones();
                    HighlightControlNode(ToolBoxContainer);
                    i = 0;
                    SetQuitSkipConfiguration();
                }
            }
        }
    }

    public override void _Ready()
    {
        Title = GetNode<RichTextLabel>("%Title");
        Body  = GetNode<RichTextLabel>("%Body");

        YesNoConfiguration = GetNode<Control>("%YesNoConfiguration");
        QuitSkipNextConfig = GetNode<Control>("%QuitSkipNextConfiguration");
        SkipContainer      = GetNode<Control>("%SkipContainer");
        NextContainer      = GetNode<Control>("%NextContainer");

        ExclusionZoneContainer.RemoveChild(ExclusionCircle);
        ExclusionZoneContainer.RemoveChild(ExclusionSquare);
    }

    public override void _Process(double delta)
    {

    }


    private void SetTitleText(string text)
    {
        Title.Text = $"[center]{text}[/center]";
    }
    private void SetBodyText(string text)
    {
        Body.Text = $"[center]{text}[/center]";
    }

    private void SetTutorialPopupCenter()
    {
        TutorialPopup.SetAnchorsAndOffsetsPreset(LayoutPreset.Center,LayoutPresetMode.KeepSize);
    }
    private void SetTutorialPopupTopRight()
    {
        TutorialPopup.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight, LayoutPresetMode.KeepSize);
    }

    private void SetQuitSkipConfiguration()
    {
        YesNoConfiguration.Visible = false;
        NextContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        SkipContainer.Visible = true;

    }
    private void SetQuitNextConfiguration()
    {
        YesNoConfiguration.Visible = false;
        SkipContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        NextContainer.Visible = true;
    }
    private void SetYesNoConfiguration()
    {
        YesNoConfiguration.Visible = true;
        QuitSkipNextConfig.Visible = false;
    }

    private void HighlightControlNode(Control control,
        float allMargins, float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNode(control, allMargins, allMargins, allMargins, allMargins, customXOffset, customYOffset);
    }
    private void HighlightControlNode(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNodeWithCustomSize(control,
            marginTop, marginRight, marginBotton, marginLeft,
            customXOffset, customYOffset,
            control.Size.X, control.Size.Y);
    }
    private void HighlightControlNodeWithCustomSize(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }
    private void HighlightControlNodeWithCustomSize(Node2D control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }

    private void ClearExclusionZones()
    {
        foreach (var child in ExclusionZoneContainer.GetChildren())
        {
            child.QueueFree();
        }

    }


    private void OnYesButtonPress()
    {
        // Replace with function body.
    }
    private void OnNoButtonPress()
    {
        // Replace with function body.
    }
    private void OnQuitButtonPress()
    {
        // Replace with function body.
    }
    private void OnNextButtonPress()
    {
        // Replace with function body.
    }
    private void OnSkipButtonPress()
    {
        // Replace with function body.
    }

}
