using CAP_Core.ExternalPorts;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ConnectAPIC.Scenes.ExternalPorts
{
    public partial class ExternalPortView : Node2D
    {
    
        [Export] public Texture2D InputTexture { set; get; }
        [Export] public Texture2D OutputTexture { set; get; }

        public ExternalPortViewModel ViewModel { get; set; }


        List<PointLight2D> lightContainer;
        TextureRect currentTexture;
        RichTextLabel InfoLabel;
        bool isInput = false;
        bool lightsOn = true;
        float pulseValue = 0;
        float minPulseEnergy = 0.8f;
        float maxPulseEnergy = 1.2f;


        public ExternalPortView(ExternalPortViewModel viewModel) {
            ViewModel = viewModel;

            ViewModel.LightChanged += (object sender, bool e) =>
            {
                if (isInput) {
                    if (e)
                        TurnOn();
                    else
                        TurnOff();
                }
            };

            viewModel.PowerChanged += (object sender, string e) =>
            {
                if (!isInput) DisplayPower(e);
            };
        }

        public override void _Ready()
        {
            currentTexture = GetChild<TextureRect>(0);

            lightContainer = new List<PointLight2D>();
            foreach (PointLight2D light in GetChild(1).GetChildren())
            {
                lightContainer.Add(light);
            }

            SetAsOutput();
        }

        public override void _Process(double delta)
        {
            if (lightsOn && isInput)
            {
                Pulsate(delta);
            }
        }


        public ExternalPortView SetAsOutput()
        {
            isInput = false;
            currentTexture.Texture = OutputTexture;
            TurnOff();

            return this;
        }

        public ExternalPortView SetAsInput(float red, float green, float blue, float alpha = 1)
        {
            isInput = true;
            lightsOn = false;
            currentTexture.Texture = InputTexture;
            pulseValue = (float)(new Random().NextDouble() * 10);

            TurnOff();
            SetLightColor(red, green, blue, alpha);

            return this;
        }

        public ExternalPortView TurnOff()
        {
            if (lightsOn)
            {
                lightContainer.ForEach((PointLight2D x) => x.Enabled = false);
                lightsOn = false;
            }

            return this;
        }

        public ExternalPortView TurnOn()
        {
            if (isInput)
            {
                lightContainer.ForEach((PointLight2D x) => x.Enabled = true);
                lightsOn = true;
            }

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
        public ExternalPortView SetLightColor(float red, float green, float blue, float alpha = 1)
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

            //TODO: can make so that inner light and outer light fluctuate differently
            lightContainer.ForEach(x => x.Energy =
            Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue))));
        }
    }
}









