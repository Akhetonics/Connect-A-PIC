using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ComponentViews
{
    public class OverlayManager
    {
        public Sprite2D OverlayRed { get; private set; } // each laser(color) is independent of the others, so they need their own overlay and shader
        public Sprite2D OverlayGreen { get; private set; }
        public Sprite2D OverlayBlue { get; private set; }
        public List<AnimationSlot> AnimationSlots { get; private set; } = new();
        public List<AnimationSlotOverlayData> AnimationSlotRawData { get; set; } = new();
        private List<Sprite2D> OverlaySprites { get; set; } = new();
        public ShaderMaterial LightOverlayShader { get; set; }
        public TextureRect GodotObject { get; }
        public int WidthInTiles { get; set; }
        public int HeightInTiles { get; set; }
        public Sprite2D OverlayBluePrint { get; set; }

        public OverlayManager(TextureRect godotObject)
        {
            GodotObject = godotObject;
        }
        public void Initialize(List<AnimationSlotOverlayData> animationSlotOverlays, int widthInTiles , int heightInTiles)
        {
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
            LightOverlayShader = new ShaderMaterial();
            LightOverlayShader.Shader = ResourceLoader.Load("res://Scripts/View/ComponentViews/LightOverlayShaded.tres").Duplicate() as Shader;
            AnimationSlotRawData = animationSlotOverlays;
            FindAndCreateLightOverlays(); // first create the overlays, then use them to create the animationSlots
            AnimationSlots = ConvertSlotOverlayDataToAnimationSlots(animationSlotOverlays);
        }

        public List<AnimationSlot> ConvertSlotOverlayDataToAnimationSlots(List<AnimationSlotOverlayData> animationSlotOverlays)
        {
            List<AnimationSlot> slots = new();
            foreach (var slotData in animationSlotOverlays)
            {
                var lightOverlayBaseTexture = ResourceLoader.Load<Texture2D>(slotData.LightFlowOverlayPath);
                slots.AddRange(CreateRGBAnimSlots(slotData.Side, slotData.FlowDirection, lightOverlayBaseTexture, new Vector2I(slotData.OffsetX, slotData.OffsetY)));
            }
            return slots;
        }
        public void HideLightVector()
        {
            OverlayRed?.Hide();
            OverlayGreen?.Hide();
            OverlayBlue?.Hide();
            ResetAllShaderParametersToZero();
        }
        private void FindOverlayBlueprint()
        {
            OverlayBluePrint = GodotObject.FindChild("Overlay", true, false) as Sprite2D;
            OverlayBluePrint ??= GodotObject.FindChild("?verlay", true, false) as Sprite2D;
            OverlayBluePrint ??= GodotObject.FindChild("*?verlay*", true, false) as Sprite2D;
        }
        protected List<AnimationSlot> CreateRGBAnimSlots(RectSide inflowSide, FlowDirection flowDirection, Godot.Texture overlayAnimTexture, Vector2I tileOffset)
        {
            return new List<AnimationSlot>()
            {
                new (LaserType.Red, tileOffset, inflowSide, flowDirection, OverlayRed, overlayAnimTexture,new Godot.Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Green,tileOffset, inflowSide, flowDirection, OverlayGreen, overlayAnimTexture, new Godot.Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Blue,tileOffset, inflowSide, flowDirection, OverlayBlue, overlayAnimTexture, new Godot.Vector2I(WidthInTiles, HeightInTiles)),
            };
        }

        private void FindAndCreateLightOverlays()
        {
            FindOverlayBlueprint();
            this.GodotObject.CheckForNull(x => OverlayBluePrint);
            OverlayBluePrint.Hide();
            OverlayRed = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Red.ToGodotColor());
            OverlayGreen = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Green.ToGodotColor());
            OverlayBlue = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Blue.ToGodotColor());

            OverlaySprites.Add(OverlayRed);
            OverlaySprites.Add(OverlayGreen);
            OverlaySprites.Add(OverlayBlue);
        }

        protected Sprite2D DuplicateAndConfigureOverlay(Sprite2D overlayDraft, Godot.Color laserColor)
        {
            var newOverlay = (Sprite2D)overlayDraft.Duplicate();
            overlayDraft.GetParent().AddChild(newOverlay);
            newOverlay.Hide();
            newOverlay.Material = LightOverlayShader.Duplicate() as ShaderMaterial;
            (newOverlay.Material as ShaderMaterial).SetShaderParameter("laserColor", laserColor);
            return newOverlay;
        }
        private void ResetAllShaderParametersToZero()
        {
            var emptyTexture = new Texture();
            foreach (AnimationSlot slot in AnimationSlots)
            {
                if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
                {
                    for (int i = 0; i < AnimationSlot.MaxShaderAnimationSlots; i++) // all parameters in the whole shader should be set to zero
                    {
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightInFlow + i, Vector4.Zero);
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightOutFlow + i, Vector4.Zero);
                        shaderMat.SetShaderParameter(ShaderParameterNames.Animation + i, emptyTexture);
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightColor, new Godot.Color(0, 0, 0));
                    }
                }
            }
        }
        public void ShowAndAssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderSlotNumber)
        {
            var inFlowPower = Math.Pow(lightAtPin.lightFieldInFlow.Magnitude, 2);
            var outFlowPower = Math.Pow(lightAtPin.lightFieldOutFlow.Magnitude, 2);
            var inFlowPowerAndPhase = new Godot.Vector4((float)inFlowPower, (float)lightAtPin.lightFieldInFlow.Phase, 0, 0);
            var outFlowPowerAndPhase = new Godot.Vector4((float)outFlowPower, (float)lightAtPin.lightFieldOutFlow.Phase, 0, 0);
            if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
            {
                if(slot?.FlowDirection == FlowDirection.In || slot?.FlowDirection == FlowDirection.Both)
                {
                    shaderMat.SetShaderParameter(ShaderParameterNames.LightInFlow + shaderSlotNumber, inFlowPowerAndPhase);
                }
                if (slot?.FlowDirection == FlowDirection.Out || slot?.FlowDirection == FlowDirection.Both)
                {
                    shaderMat.SetShaderParameter(ShaderParameterNames.LightOutFlow + shaderSlotNumber, outFlowPowerAndPhase);
                }
                shaderMat.SetShaderParameter(ShaderParameterNames.Animation + shaderSlotNumber, slot.Texture);
                shaderMat.SetShaderParameter(ShaderParameterNames.LightColor + shaderSlotNumber, slot.MatchingLaser.Color.ToGodotColor());
            }

            
            OverlayRed?.Show();
            OverlayGreen?.Show();
            OverlayBlue?.Show();
        }

    }
}
