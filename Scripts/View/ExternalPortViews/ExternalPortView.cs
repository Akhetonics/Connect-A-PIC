using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;


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
        bool pulsate = false;
        float pulseValue = 0;
        float minPulseEnergy = 0.8f;
        float maxPulseEnergy = 1.2f;


        public ExternalPortView()
        {
        }

        public void Initialize(ExternalPortViewModel viewModel)
        {
            ViewModel = viewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            ViewModel.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                //TODO: search if new one is same as 

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
            InfoLabel = GetChild<RichTextLabel>(2);
        }

        public override void _Process(double delta)
        {
            if (pulsate)
            {
                Pulsate(delta);
            }
        }

        public void SetAsOutput()
        {
            currentTexture.Texture = OutputTexture;
        }

        public void SetAsInput(float alpha = 1)
        {
            currentTexture.Texture = InputTexture;
            pulseValue = (float)(new Random().NextDouble() * 10);
            SetLightColor(alpha);
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
        public void SetLightColor(float alpha = 1)
        {
            for (int i = 0; i < lightContainer.Count; i++)
            {
                lightContainer[i].Color = new Color(ViewModel.Power.X, ViewModel.Power.Y, ViewModel.Power.Z, alpha);
            }
        }

        private void Pulsate(double delta)
        {
            pulseValue = (pulseValue + (float)delta) % (2 * Mathf.Pi);
            lightContainer[0].Energy =
            Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue)));
        }


        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExternalPortViewModel.LightIsOn))
            {
                if (ViewModel.IsInput){
                    lightContainer?.ForEach((PointLight2D x) => x.Enabled = ViewModel.LightIsOn);
                    pulsate = ViewModel.LightIsOn;
                }
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.Power))
            {
                if (ViewModel.IsInput)
                    SetLightColor();
                else
                    InfoLabel.Text = ViewModel.AllColorsPower();
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.IsInput))
            {
                if (ViewModel.IsInput)
                    SetAsInput();
                else
                    SetAsOutput();
            }
        }
    }
}
