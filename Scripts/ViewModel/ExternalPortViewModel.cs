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
    public ObservableCollection<ExternalPort> ExternalPorts { get; set; }

    public int TilePositionY { get; private set; } = -1;

    private bool _isInput;
    public bool IsInput {
        get => _isInput;
        set{
            _isInput = value;
            OnPropertyChanged();
        }
    }

    private bool _lightIsOn;
    public bool LightIsOn
    {
        get { return _lightIsOn; }
        set
        {
            _lightIsOn = value;
            OnPropertyChanged();
        }
    }

    private Vector3 _power; //rgb (0-1) values for power red/green/blue (for now it represents output power, could be input power later)
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

        Grid.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            // delete all external ports one by one 
            foreach (ExternalPort port in e.OldItems)
            {
                ExternalPorts.Remove(port);
            }
            // add the new ports
            foreach (ExternalPort port in e.NewItems)
            {
                ExternalPorts.Add(port);
            }
        };

        Grid.OnLightSwitched += (object sender, bool e) =>
        {
            LightIsOn = e;
        };

        lightCalculator.LightCalculationChanged += (object sender, LightCalculationChangeEventArgs e) =>
        {
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
            if (e.LaserInUse.Color == LightColor.Red)
            {
                // floats should be sufficient for this value
                SetPower(red: (float)(fieldOut * fieldOut));
            }
            else if (e.LaserInUse.Color == LightColor.Green)
            {
                SetPower(green: (float)(fieldOut * fieldOut));
            }
            else
            {
                SetPower(blue: (float)(fieldOut * fieldOut));
            }
        };
    }

    /// <summary>
    /// Sets power vector, used for conviniently setting power parameter
    /// </summary>
    /// <param name="red"> red power value in range [0, 1]</param>
    /// <param name="green"> green power value in range [0, 1]</param>
    /// <param name="blue"> blue power value in range [0, 1]</param>
    /// <param name="setNonZeros"> if true will only set the value which is different from 0 and leave others as they were before in power vector</param>
    public void SetPower(float red = 0, float green = 0, float blue = 0, bool setNonZeros = false) {
        if (setNonZeros)
        {
            Vector3 power = Power;
            if(red != 0) power.X = red;
            if(green != 0) power.Y = green;
            if (blue != 0) power.Z = blue;
            Power = power;
        }
        else
        {
            Power = new Vector3(red, green, blue);
        }
    }
    public string AllColorsPower()
    {
        string allUsedPowers = "";

        if (Power.X > 0.005)
        {
            allUsedPowers += $"[color=#FF6666]R: {Power.X:F2}[/color]\n";
        }
        if (Power.Y > 0.005)
        {
            allUsedPowers += $"[color=#66FF66]G: {Power.Y:F2}[/color]\n";
        }
        if (Power.Z > 0.005)
        {
            allUsedPowers += $"[color=#6666FF]B: {Power.Z:F2}[/color]";
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

    private void PortsSwitched(object sender, int e)
    {
        int portIndex = Grid.ExternalPorts.IndexOf(Grid.ExternalPorts.FirstOrDefault(exPort => exPort.TilePositionY == e));

        if (portIndex == -1)
        {
            //TODO: print some error
            return;
        }

        if (Grid.ExternalPorts[portIndex] is ExternalInput oldExternalInput)
        {
            //TODO: need to refactor this after implementing right click menu
            if (oldExternalInput.LaserType == LaserType.Red)
            {
                Grid.ExternalPorts[portIndex] = new ExternalInput(oldExternalInput.PinName, LaserType.Green, oldExternalInput.TilePositionY, oldExternalInput.InFlowPower);
            }
            else if (oldExternalInput.LaserType == LaserType.Green)
            {
                Grid.ExternalPorts[portIndex] = new ExternalInput(oldExternalInput.PinName, LaserType.Blue, oldExternalInput.TilePositionY, oldExternalInput.InFlowPower);
            }
            else
            {
                Grid.ExternalPorts[portIndex] = new ExternalOutput(oldExternalInput.PinName, oldExternalInput.TilePositionY);
            }
        }
        else
        {
            ExternalOutput oldOutput = (ExternalOutput)Grid.ExternalPorts[portIndex];
            Grid.ExternalPorts[portIndex] = new ExternalInput(oldOutput.PinName, LaserType.Red, oldOutput.TilePositionY, 1);
        }

        //TODO: remove this after debugging
        foreach (var port in Grid.ExternalPorts)
        {
            if (port is ExternalInput)
            {
                Debug.Print("Input: " + port.PinName + " " + port.TilePositionY);
            }
            else
            {
                Debug.Print("Output: " + port.PinName + " " + port.TilePositionY);
            }
        }

    }

}

