using CAP_Core.ExternalPorts;
using ConnectAPIC.Scenes.InteractionOverlay;
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
        Control FlipContainer;
        bool pulsate = false;
        float pulseValue = 0;
        float minPulseEnergy = 0.8f;
        float maxPulseEnergy = 1.2f;
        bool mouseInClickArea = false;

        public void Initialize(ExternalPortViewModel viewModel)
        {
            ViewModel = viewModel;

            currentTexture = GetNode<TextureRect>("%CurrentTexture");
            FlipContainer = GetNode<Control>("%FlipContainer");
            lightContainer = new List<PointLight2D>();
            foreach (PointLight2D light in GetNode<Node2D>("%LightContainer").GetChildren())
            {
                lightContainer.Add(light);
            }
            InfoLabel = GetNode<RichTextLabel>("%Label");

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if (ViewModel.IsInput)
            {
                SetAsInput();
            }
            else
            {
                SetAsOutput();
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton
                && mouseButton.Pressed
                && mouseButton.ButtonIndex == MouseButton.Left
                && mouseInClickArea
                && InteractionOverlayController.ClickingAllowed)
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

        public void SetSide()
        {
            if (ViewModel.IsLeftPort)
            {
                FlipContainer.Scale = new Vector2(1, 1);
                InfoLabel.TextDirection = Control.TextDirection.Ltr;
            }
            else
            {
                FlipContainer.Scale = new Vector2(-1, 1);
                InfoLabel.TextDirection = Control.TextDirection.Rtl;
            }
        }

        public void SetAsOutput()
        {
            currentTexture.Texture = OutputTexture;
            InfoLabel.Text = ViewModel.AllColorsPower();
            InfoLabel.Visible = true;
            SetLight(false);
            SetSide();
        }

        public void SetAsInput(float alpha = 1)
        {
            currentTexture.Texture = InputTexture;
            pulseValue = (float)(new Random().NextDouble() * 10);
            InfoLabel.Visible = false;
            SetLightColor(alpha);
            SetLight(ViewModel.IsLightOn);
            SetSide();
        }

        public void SetLightColor(float alpha = 1)
        {
            System.Drawing.Color color = ViewModel.Color.Color.ToColor();
            float multiplier = ViewModel.Power.Length();
            foreach (var light in lightContainer)
            {
                light.Color = new Color(multiplier * color.R / 255, multiplier * color.G / 255, multiplier * color.B / 255, alpha);
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
                else if (ViewModel.IsLightOn)
                {
                    InfoLabel.Text = ViewModel.AllColorsPower();
                }
                else
                {
                    InfoLabel.Text = "";
                }
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.Power)
                  || e.PropertyName == nameof(ExternalPortViewModel.Color))
            {
                if (ViewModel.IsInput)
                {
                    SetLightColor();
                }
                else
                    InfoLabel.Text = ViewModel.AllColorsPower();
            }
            else if (e.PropertyName == nameof(ExternalPortViewModel.PortModel))
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


