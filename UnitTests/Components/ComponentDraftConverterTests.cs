using Xunit;
using Moq;
using CAP_Contracts.Logger;
using System.Collections.Generic;
using Shouldly;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_Core.Tiles;
using CAP_Core.ExternalPorts;

namespace UnitTests.Components
{
    public class ComponentDraftConverterTests
    {
        private readonly Mock<ILogger> mockLogger;

        public ComponentDraftConverterTests()
        {
            mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void TestCreateComponentFunctionFromDraft()
        {
            // Arrange
            ComponentDraftConverter converter = new ComponentDraftConverter(mockLogger.Object);
            var componentDraft = new ComponentDraft
            {
                Identifier = "Straight",
                NazcaFunctionParameters = "",
                NazcaFunctionName = "placeCell_StraightWG",
                SceneResPath = "res://Scenes/Components/Straight/StraightWaveGuide.tscn",
                WidthInTiles = 1,
                HeightInTiles = 1,
                Pins = new List<PinDraft>
                {
                    new PinDraft { Number = 0, Name = "west", MatterType = CAP_Core.Components.MatterType.Light, Side = RectSide.Left, PartX = 0, PartY = 0 },
                    new PinDraft { Number = 1, Name = "east", MatterType = CAP_Core.Components.MatterType.Light, Side = RectSide.Right, PartX = 0, PartY = 0 }
                },
                SMatrices = new List<WaveLengthSpecificSMatrix>
                {
                    new()
                    {
                        WaveLength = LaserType.Red.WaveLengthInNm,
                        Connections = new List<Connection>
                        {
                            new Connection { FromPinNr = 0, ToPinNr = 1, Real = 1.0, Imaginary = 2.0, Formula = "Div(1,Add(PIN0)+1)" },
                            new Connection { FromPinNr = 1, ToPinNr = 0, Real = 1.0, Imaginary = 1.0 }
                        }
                    }
                },
                Sliders = new List<SliderDraft> {
                    new SliderDraft { GodotSliderLabelName = "Label", GodotSliderName  = "GodotSlider" , SliderNumber = 2 , MaxVal = 1 , MinVal = 0.1, Steps = 100, Type = SliderTypes.LinearSlider },
                    new SliderDraft { GodotSliderLabelName = "Label1", GodotSliderName  = "GodotSlider2" , SliderNumber = 3}
                }
            };

            // Act
            var components = converter.ToComponentModels(new List<ComponentDraft> { componentDraft });


            // Assert
            components.ShouldNotBeEmpty();
            components.Count.ShouldBe(1);
            var component = components.First();
            component.Identifier.ShouldBe(componentDraft.Identifier);
            var sMatrix = component.LaserWaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm];
            sMatrix.NonLinearConnections.Count.ShouldBe(1);
            sMatrix.PinReference.Count.ShouldBe(4); //because we have 2 pins which each consist of an output and input Pin-ID.
            sMatrix.GetNonNullValues().Count.ShouldBe(2);
            component.Sliders.ContainsKey(2).ShouldBe(true);
            component.Sliders.ContainsKey(3).ShouldBe(true);
            componentDraft.Sliders.Single(s => s.SliderNumber == 2).GodotSliderName.ShouldBe("GodotSlider");
            var minorDefinedSlider = componentDraft.Sliders.Single(s => s.SliderNumber == 3);
            minorDefinedSlider.MaxVal.ShouldBe(1);
            minorDefinedSlider.MinVal.ShouldBe(0);
            minorDefinedSlider.Steps.ShouldBe(100);
        }
    }
}