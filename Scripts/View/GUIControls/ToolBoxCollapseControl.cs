using Godot;
using System;
using System.Drawing;

public partial class ToolBoxCollapseControl : Control
{
    [Export] public float CollapseSpeed { get; set; } = 0.2f;
    [Export] public int SlideOffset { get; set; } = -34;

    private bool _isCollapsed = false;
    private float _targetPosition = 0;
    private float _originalToolBoxSize;
    private MarginContainer _marginContainer;


    public override void _Ready()
    {
        _marginContainer = GetChild<MarginContainer>(0);
        _originalToolBoxSize = Size.Y;
    }

    public override void _Process(double delta)
    {
        float newY = Mathf.Lerp(_marginContainer.Position.Y, _targetPosition, CollapseSpeed);

        _marginContainer.SetPosition(new Vector2(_marginContainer.Position.X, newY));
    }
    

    private void OnToggleButtonPressed(bool toggled_on)
    {
        _isCollapsed = toggled_on;

		if (_isCollapsed)
		{
			_targetPosition = _originalToolBoxSize + SlideOffset;
			MouseFilter = MouseFilterEnum.Ignore;
		}
		else
		{
			_targetPosition = 0;
            MouseFilter = MouseFilterEnum.Stop;
        }
	}


}

