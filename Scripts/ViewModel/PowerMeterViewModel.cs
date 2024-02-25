using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel
{
    public class PowerMeterViewModel
    {
        public event EventHandler<string> PowerChanged;
        public GridManager Grid { get; }
        public int TilePositionY { get; }
        public double PowerRed { get; set; }
        public double PowerGreen { get; set; }
        public double PowerBlue { get; set; }
        public string AllColorsPower()
        {
            string allUsedPowers = "";

            if (PowerRed > 0.005)
            {
                allUsedPowers += $"[color=#FF6666]R: {PowerRed:F2}[/color]\n";
            }
            if (PowerGreen > 0.005)
            {
                allUsedPowers += $"[color=#66FF66]G: {PowerGreen:F2}[/color]\n";
            }
            if (PowerBlue > 0.005)
            {
                allUsedPowers += $"[color=#6666FF]B: {PowerBlue:F2}[/color]";
            }

            // Removes the trailing newline character if any colors were added
            return allUsedPowers.TrimEnd('\n');
        }
        private void ResetPowers()
        {
            PowerRed = 0;
            PowerGreen = 0;
            PowerBlue = 0;
            PowerChanged?.Invoke(this, AllColorsPower());
        }
        public PowerMeterViewModel(GridManager grid, int tilePositionY, LightCalculationService lightCalculator)
        {
            lightCalculator.LightCalculationChanged += (object sender, LightCalculationChangeEventArgs e) =>
            {
                var touchingComponent = grid.GetComponentAt(0,TilePositionY);
                if (touchingComponent == null) {
                    ResetPowers();
                    return;
                };
                var offsetY = TilePositionY - touchingComponent.GridYMainTile;
                var touchingPin = touchingComponent.PinIdLeftOut(0, offsetY);
                if(touchingPin == null)
                {
                    ResetPowers();
                    return;
                };
                var fieldOut = e.LightFieldVector[(Guid)touchingPin].Magnitude;
                if(e.LaserInUse.Color == LightColor.Red)
                {
                    PowerRed = fieldOut * fieldOut;
                } else if (e.LaserInUse.Color == LightColor.Green)
                {
                    PowerGreen = fieldOut * fieldOut;
                } else
                {
                    PowerBlue = fieldOut * fieldOut;
                }
                PowerChanged?.Invoke(this, AllColorsPower());
            };

            Grid = grid;
            TilePositionY = tilePositionY;
            PowerChanged?.Invoke(this, AllColorsPower());
        }

    }
}
