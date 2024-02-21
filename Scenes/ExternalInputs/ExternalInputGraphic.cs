using CAP_Core.ExternalPorts;
using Godot;
using MathNet.Numerics.Distributions;
using System;


namespace ConnectAPIC.Scenes.ExternalInputs
{
    public partial class ExternalInputGraphic : Node2D
    {
        [Export] public Texture2D lightOnTexture { set; get; }
        [Export] public Texture2D lightOffTexture { set; get; }
        [Export] public Color lightColor { set; get; }
        [Export] public Button button { set; get; }

        public TextureRect currentTexture { get; set; }
        public PointLight2D light { get; set; }

        bool lightsOn = false;
        float pulseValue = 0;
        float minPulseEnergy = 0.95f;
        float maxPulseEnergy = 1.2f;

        public override void _Ready()
        {
            pulseValue = (float)(new Random().NextDouble() * 10);
            currentTexture = GetChild<TextureRect>(0);
            light = GetChild<PointLight2D>(1);
            light.Color = lightColor;

            if (button.ButtonPressed)
            {
                TurnOn();
            }
            else 
            {
                TurnOff();
            }

            button.Toggled += (bool isToggled) => OnToggleButtonPressed(isToggled);

        }

        public override void _Process(double delta)
        {
            if(lightsOn) {
                Pulsate(delta);
            }
        }


        private void OnToggleButtonPressed(bool isToggled)
        {
            if (isToggled)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        private void TurnOff()
        {
            currentTexture.Texture = lightOffTexture;
            light.Visible = false;
            lightsOn = false;
        }

        private void TurnOn()
        {
            currentTexture.Texture = lightOnTexture;
            light.Visible = true;
            lightsOn = true;
        }

        private void Pulsate(double delta)
        {
            pulseValue = (pulseValue + (float)delta) % (2 * Mathf.Pi);

            light.Energy = Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue)));
        }
    }
}





