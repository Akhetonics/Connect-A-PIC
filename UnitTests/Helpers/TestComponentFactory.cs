using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles;
using System.Numerics;

namespace UnitTests
{
    public class TestComponentFactory
    {
        public static string StraightWGJsonString { get; set; } = @"
        {
            ""fileFormatVersion"": 1,
            ""identifier"": ""Straight"",
            ""nazcaFunctionParameters"": """",
            ""nazcaFunctionName"": ""placeCell_StraightWG"",
            ""sceneResPath"": ""res://Scenes/Components/Straight/StraightWaveGuide.tscn"",
            ""deltaLength"": 0,
            ""widthInTiles"": 1,
            ""heightInTiles"": 1,
            ""pins"": [
                {
                    ""number"": 0,
                    ""name"": ""west"",
                    ""matterType"": 1,
                    ""side"": 2,
                    ""partX"": 0,
                    ""partY"": 0
                },
                {
                    ""number"": 1,
                    ""name"": ""east"",
                    ""matterType"": 1,
                    ""side"": 0,
                    ""partX"": 0,
                    ""partY"": 0
                }
            ],
            ""sMatrices"": [
                {
                    ""waveLength"" : 1550,
                    ""connections"": [
                    {
                        ""fromPinNr"": 0,
                        ""toPinNr"": 1,
                        ""real"": 1.0,
                        ""imaginary"" : 2.0
                    },
                    {
                        ""fromPinNr"": 1,
                        ""toPinNr"": 0,
                        ""real"": 1.0,
                        ""imaginary"" : 1.0
                    }
                    ]
                },
                {
                    ""waveLength"" : 1310,
                    ""connections"": [
                    ]
                },
                {
                    ""waveLength"" : 980,
                    ""connections"": [
                    ]
                }
            ],
            ""overlays"":[
            {
                ""overlayAnimTexturePath"" : ""res://dummyPath.png"",
                ""rectSide"" : 2,
                ""tileOffsetX"" : 0,
                ""tileOffsetY"" : 1
            }],
            ""sliders"": [
		        {
			        ""sliderNumber"" : 0,
			        ""godotSliderName"" : ""JumpRatioSlider"",
			        ""godotSliderLabelName"" : ""JumpRatioSliderLabel"",
			        ""minVal"" : 0.0,
			        ""maxVal"" : 1.0,
			        ""steps"" : 100,
			        ""type"" : 0
		        }
            ]
            }";
        public static Component CreateStraightWaveGuide()
        {
            int widthInTiles = 1;
            int heightInTiles = 1;

            Part[,] parts = new Part[widthInTiles, heightInTiles];

            parts[0, 0] = new Part(new List<Pin>() {
                new ("west0",0, MatterType.Light, RectSide.Left),
                new ("east0",1, MatterType.Light, RectSide.Right)
            });


            var leftIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var rightOut = parts[0, 0].GetPinAt(RectSide.Right).IDOutFlow;
            var rightIn = parts[0, 0].GetPinAt(RectSide.Right).IDInFlow;
            var leftOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;

            var allPins = Component.GetAllPins(parts).SelectMany(p => new[] { p.IDInFlow, p.IDOutFlow }).ToList();
            var matrixRed = new SMatrix(allPins);
            // set the connections
            matrixRed.SetValues(new(){
                { (leftIn, rightOut), 1 },
                { (rightIn, leftOut), 1 },
            });
            var connections = new Dictionary<int,SMatrix>
            {
                { StandardWaveLengths.RedNM, matrixRed},
                { StandardWaveLengths.GreenNM, matrixRed},
                { StandardWaveLengths.BlueNM, matrixRed},
            };

            return new Component(connections, "placeCell_StraightWG", "", parts, 0, "Straight", DiscreteRotation.R0);
        }

        public static Component CreateDirectionalCoupler()
        {
            int widthInTiles = 2;
            int heightInTiles = 2;

            Part[,] parts = new Part[widthInTiles, heightInTiles];


            parts[0, 0] = new Part(new List<Pin>() { new Pin("west0", 0, MatterType.Light, RectSide.Left) });
            parts[1, 0] = new Part(new List<Pin>() { new Pin("east0",1, MatterType.Light, RectSide.Right) });
            parts[1, 1] = new Part(new List<Pin>() { new Pin("east1",2, MatterType.Light, RectSide.Right) });
            parts[0, 1] = new Part(new List<Pin>() { new Pin("west1",3, MatterType.Light, RectSide.Left) });

            // setting up the connections
            var leftUpIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var leftUpOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;
            var leftDownIn = parts[0, 1].GetPinAt(RectSide.Left).IDInFlow;
            var leftDownOut = parts[0, 1].GetPinAt(RectSide.Left).IDOutFlow;
            var rightUpIn = parts[1, 0].GetPinAt(RectSide.Right).IDInFlow;
            var rightUpOut = parts[1, 0].GetPinAt(RectSide.Right).IDOutFlow;
            var rightDownIn = parts[1, 1].GetPinAt(RectSide.Right).IDInFlow;
            var rightDownOut = parts[1, 1].GetPinAt(RectSide.Right).IDOutFlow;
            
            var allPins = Component.GetAllPins(parts).SelectMany(p => new[] { p.IDInFlow, p.IDOutFlow }).ToList();
            var matrixRed = new SMatrix(allPins);
            // set the connections
            matrixRed.SetValues(new(){
                { (leftUpIn, rightUpOut), 0.5f },
                { (leftUpIn, rightDownOut), 0.5f },
                { (leftDownIn, rightUpOut), 0.5f },
                { (leftDownIn, rightDownOut), 0.5f },
                { (rightUpIn, leftUpOut), 0.5f },
                { (rightUpIn, leftDownOut), 0.5f },
                { (rightDownIn, leftUpOut), 0.5f },
                { (rightDownIn, leftDownOut), 0.5f },
                
            });
            var connections = new Dictionary<int, SMatrix>
            {
                {StandardWaveLengths.RedNM, matrixRed},
                {StandardWaveLengths.GreenNM, matrixRed},
                {StandardWaveLengths.BlueNM, matrixRed},
            };
            return new Component(connections, "placeCell_DirectionalCoupler", "", parts, 0,"DirectionalCoupler", DiscreteRotation.R0);
        }

    }
}
