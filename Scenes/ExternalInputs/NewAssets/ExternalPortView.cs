using CAP_Core.ExternalPorts;
using Godot;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ConnectAPIC.Scenes.ExternalInputs
{
    public partial class ExternalPortView : Node2D
    {
    
        [Export] public Texture2D InputTexture { set; get; }
        [Export] public Texture2D OutputTexture { set; get; }


        private List<PointLight2D> lightContainer;
        private TextureRect currentTexture;
        bool isInput = false;
        bool lightsOn = true;
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

            pulseValue = (float)(new Random().NextDouble() * 10);

            SetAsOutput();
            SetAsInput(true, 0, 1, 0);
        }

        public override void _Process(double delta)
        {
            if (lightsOn && isInput)
            {
                Pulsate(delta);
            }
        }

        public void SetAsOutput()
        {
            isInput = false;
            currentTexture.Texture = OutputTexture;
            TurnOff();
        }

        public void SetAsInput(bool lightsOn, float red, float green, float blue, float alpha = 1)
        {
            isInput = true;
            currentTexture.Texture = InputTexture;
            SetLightColor(red, green, blue, alpha);

            if (lightsOn)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        public void TurnOff()
        {
            if (lightsOn)
            {
                lightContainer.ForEach((PointLight2D x) => x.Enabled = false);
                lightsOn = false;
            }
        }

        public void TurnOn()
        {
            if (!lightsOn && isInput)
            {
                lightContainer.ForEach((PointLight2D x) => x.Enabled = true);
                lightsOn = true;
            }
        }

        /// <summary>
        /// sets color of input pin according to RGBA colors (in range 0-1)
        /// </summary>
        /// <param name="red">float value between 0-1 indicating red</param>
        /// <param name="green">float value between 0-1 indicating green</param>
        /// <param name="blue">float value between 0-1 indicating blue</param>
        /// <param name="alpha">float value between 0-1 indicating transparency</param>
        public void SetLightColor(float red, float green, float blue, float alpha = 1)
        {
            for (int i = 0; i < lightContainer.Count; i++)
            {
                lightContainer[i].Color = new Color(red, green, blue, alpha);
            }
        }

        private void Pulsate(double delta)
        {
            pulseValue = (pulseValue + (float)delta) % (2 * Mathf.Pi);

            //TODO: can make so that inner light and outer light fluctuate differently
            lightContainer.ForEach(x => x.Energy =
            Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue))));
        }
    }
}









