using CAP_Core.ExternalPorts;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ConnectAPic.LayoutWindow.GameManager;


namespace ConnectAPIC.Scenes.ExternalPorts
{
    public partial class ExternalPortScene : Node2D
    {

        [Export] public Texture2D InputTexture { set; get; }
        [Export] public Texture2D OutputTexture { set; get; }

        public int PortPositionY { get; private set; } = -1;

        public bool IsInput { get; private set; } = false;

        public bool LightsOn
        {
            get => lightsOn;
            set
            {
                lightContainer?.ForEach((PointLight2D x) => x.Enabled = value);
                lightsOn = value;
            }
        }
        bool lightsOn;

        List<PointLight2D> lightContainer;
        TextureRect currentTexture;
        RichTextLabel InfoLabel;
        float pulseValue = 0;
        float minPulseEnergy = 0.8f;
        float maxPulseEnergy = 1.2f;

        public override void _Ready()
        {
            currentTexture = GetChild<TextureRect>(0);
            lightContainer = new List<PointLight2D>();
            foreach (PointLight2D light in GetChild(1).GetChildren())
            {
                lightContainer.Add(light);
            }
            InfoLabel = GetChild<RichTextLabel>(2);
        }

        public override void _Process(double delta)
        {
            if (lightsOn && IsInput)
            {
                Pulsate(delta);
            }
        }

        public ExternalPortScene SetPortPositionY(int positionY)
        {
            PortPositionY = positionY;
            return this;
        }

        public ExternalPortScene SetAsOutput()
        {
            IsInput = false;
            LightsOn = false;
            currentTexture.Texture = OutputTexture;
            return this;
        }

        public ExternalPortScene SetAsInput(float red, float green, float blue, float alpha = 1)
        {
            IsInput = true;
            LightsOn = false;
            currentTexture.Texture = InputTexture;
            pulseValue = (float)(new Random().NextDouble() * 10);

            SetLightColor(red, green, blue, alpha);

            return this;
        }


        /// <summary>
        /// sets color of input pin according to RGBA colors (in range 0-1)
        /// and returns ExternalPortView with the assigned light color
        /// </summary>
        /// <param name="red">float value between 0-1 indicating red</param>
        /// <param name="green">float value between 0-1 indicating green</param>
        /// <param name="blue">float value between 0-1 indicating blue</param>
        /// <param name="alpha">float value between 0-1 indicating transparency</param>
        /// <returns>ExternalPortView - with lighting of the given type</returns>
        public ExternalPortScene SetLightColor(float red, float green, float blue, float alpha = 1)
        {
            for (int i = 0; i < lightContainer.Count; i++)
            {
                lightContainer[i].Color = new Color(red, green, blue, alpha);
            }

            return this;
        }

        private void DisplayPower(string labelText)
        {
            InfoLabel.Text = labelText;
        }

        private void Pulsate(double delta)
        {
            pulseValue = (pulseValue + (float)delta) % (2 * Mathf.Pi);
            lightContainer[0].Energy =
            Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue)));
        }
    }
}

