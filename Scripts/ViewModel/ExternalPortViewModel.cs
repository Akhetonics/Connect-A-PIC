using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using Godot;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;


public partial class ExternalPortViewModel : Node, INotifyPropertyChanged
{
    public GridManager Grid { get; }
    public LightCalculationService LightCalculator { get; }

    public int TilePositionY { get; private set; } = -1;

    private bool _isInput;
    public bool IsInput {
        get => _isInput;
        set{
            _isInput = value;
            OnPropertyChanged();
        }
    }

    private bool _isLightOn;
    public bool IsLightOn
    {
        get { return _isLightOn; }
        set
        {
            _isLightOn = value;
            OnPropertyChanged();
        }
    }

    private Vector3 _power;
    public Vector3 Power
    {
        get => _power;
        set
        {
            _power = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ExternalPortViewModel(GridManager grid, int tilePositionY, LightCalculationService lightCalculator)
    {
        LightCalculator = lightCalculator;
        TilePositionY = tilePositionY;
        Power = Vector3.Zero;
        Grid = grid;

        Grid.OnLightSwitched += (object sender, bool e) =>
        {
            IsLightOn = e;
            if (!IsLightOn)
            {
                //for some reason when light turns off LightCalculationChanged signal isn't emitted
                ResetPowers();
            }
        };

        lightCalculator.LightCalculationChanged += (object sender, LightCalculationChangeEventArgs e) =>
        {
            if (IsInput) return;
            var touchingComponent = grid.GetComponentAt(0, TilePositionY);
            if (touchingComponent == null)
            {
                ResetPowers();
                return;
            };
            var offsetY = TilePositionY - touchingComponent.GridYMainTile;
            var touchingPin = touchingComponent.PinIdLeftOut(0, offsetY);
            if (touchingPin == null)
            {
                ResetPowers();
                return;
            };
            var fieldOut = e.LightFieldVector[(Guid)touchingPin].Magnitude;
            var power = _power;
            if (e.LaserInUse.Color == LightColor.Red)
            {
                // floats should be sufficient for this value
                power.X = (float)(fieldOut * fieldOut);
            }
            else if (e.LaserInUse.Color == LightColor.Green)
            {
                power.Y = (float)(fieldOut * fieldOut);
            }
            else
            {
                power.Z = (float)(fieldOut * fieldOut);
            }
            Power = power;
        };
    }


    public string AllColorsPower()
    {
        string allUsedPowers = "";

        if (Power.X > 0.005)
        {
            allUsedPowers += $"[color=#FF4444]R: {Power.X:F2}[/color]\n";
        }
        if (Power.Y > 0.005)
        {
            allUsedPowers += $"[color=#44FF44]G: {Power.Y:F2}[/color]\n";
        }
        if (Power.Z > 0.005)
        {
            allUsedPowers += $"[color=#4444FF]B: {Power.Z:F2}[/color]";
        }

        // Removes the trailing newline character if any colors were added
        return allUsedPowers.TrimEnd('\n');
    }
    private void ResetPowers()
    {
        Power = Vector3.Zero;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

