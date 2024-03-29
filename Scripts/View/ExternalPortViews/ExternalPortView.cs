using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConnectAPIC.Scenes.RightClickMenu;


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
        bool mouseInClickArea = false;

        public void Initialize(ExternalPortViewModel viewModel)
        {
            ViewModel = viewModel;

            currentTexture = GetChild<TextureRect>(0);
            lightContainer = new List<PointLight2D>();
            foreach (PointLight2D light in GetChild(1).GetChildren())
            {
                lightContainer.Add(light);
            }
            InfoLabel = GetChild<RichTextLabel>(2);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton
                && mouseButton.Pressed
                && mouseButton.ButtonIndex == MouseButton.Left
                && mouseInClickArea)
            {
                ViewModel.InvokeClicked();
            }
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
            InfoLabel.Text = ViewModel.AllColorsPower();
            InfoLabel.Visible = true;
            SetLight(false);
        }

        public void SetAsInput(float alpha = 1)
        {
            currentTexture.Texture = InputTexture;
            pulseValue = (float)(new Random().NextDouble() * 10);
            InfoLabel.Visible = false;
            SetLightColor(alpha);
            SetLight(ViewModel.IsLightOn);
        }

        public void SetLightColor(float alpha = 1)
        {
            foreach (var light in lightContainer)
            {
                light.Color = new Color(ViewModel.Power.X, ViewModel.Power.Y, ViewModel.Power.Z, alpha);
            }
        }

        private void Pulsate(double delta)
        {
            pulseValue = (pulseValue + (float)delta) % (2 * Mathf.Pi);
            lightContainer[0].Energy =
            Mathf.Lerp(minPulseEnergy, maxPulseEnergy, Mathf.Abs(Mathf.Sin(pulseValue)));
        }

        private void SetLight(bool isLightOn)
        {
            lightContainer.ForEach(x => x.Enabled = isLightOn);

            pulsate = isLightOn;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExternalPortViewModel.IsLightOn))
            {
                if (ViewModel.IsInput)
                {
                    SetLight(ViewModel.IsLightOn);
                }
                else
                {
                    InfoLabel.Text = ViewModel.AllColorsPower();
                }
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.Power))
            {
                if (ViewModel.IsInput){
                    SetLightColor();
                }
                else
                    InfoLabel.Text = ViewModel.AllColorsPower();
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.IsInput))
            {
                if (ViewModel.IsInput)
                {
                    SetAsInput();
                }
                else
                    SetAsOutput();
            }
        }

        private void OnMouseEnteredRightClickArea()
        {
            mouseInClickArea = true;
        }

        private void OnMouseExitedRightClickArea()
        {
            mouseInClickArea = false;
        }
    }
}


