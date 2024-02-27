using CAP_Contracts.Logger;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests
{
    public partial class ComponentDraftFileReaderTests
    {

        [Fact]
        public async Task TestReadWrite()
        {
            // test if first writing and then reading of a componentDraft works as expected
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            var originalComponentDraft = new ComponentDraft
            {
                FileFormatVersion = 1,
                Identifier = "test",
                NazcaFunctionName = "test",
                NazcaFunctionParameters = "",
                SceneResPath = "res://Scenes/Components/DirectionalCoupler.tscn",
                WidthInTiles = 1,
                HeightInTiles = 1,
                Pins = new List<PinDraft>
                {
                    new PinDraft()
                    {
                        MatterType = CAP_Core.Components.MatterType.Light,
                        Name = "west0",
                        Number = 0,
                        PartX = 0,
                        PartY = 0,
                        Side = RectSide.Left,
                    },
                    new PinDraft()
                    {
                        MatterType = CAP_Core.Components.MatterType.Light,
                        Name = "west0",
                        Number = 1,
                        PartX = 0,
                        PartY = 0,
                        Side = RectSide.Right,
                    }
                },
                SMatrices = new List<WaveLengthSpecificSMatrix> {
                    new WaveLengthSpecificSMatrix() {
                        WaveLength = 1550,
                        Connections = new List<Connection>()
                        {
                            new Connection()
                            {
                                FromPinNr = 0,
                                ToPinNr = 1,
                                Formula = "PIN0 +1",
                                Imaginary = 0
                            },
                            new Connection()
                            {
                                FromPinNr = 1,
                                ToPinNr = 0,
                                Real = 0.9,
                                Imaginary = 0.3
                            }
                        }
                    } 
                },
                Overlays = new List<Overlay>
                {
                    new ()
                    {
                        OverlayAnimTexturePath = "res://Scenes/Components/DirectionalCoupler.tscn",
                        FlowDirection = FlowDirection.In,
                        TileOffsetX = 0,
                        TileOffsetY = 0,
                    }
                },
            };

            // Act
            var reader = new ComponentDraftFileReader(new FileDataAccessor());
            await reader.Write(tempFilePath, originalComponentDraft);
            bool fileIsUsed = true;
            int counter = 0;
            ComponentDraft? readComponentDraft = null;
            while (fileIsUsed)
            {
                try
                {
                    var readResult = reader.TryReadJson(tempFilePath);
                    
                    if(readResult.error != null)
                    {
                        throw new IOException(readResult.error);
                    }
                    readComponentDraft = readResult.draft;
                    fileIsUsed = false;
                } catch (IOException ex)
                {
                    await Task.Delay(10);
                    counter++;
                    if (counter > 100)
                        throw ex;
                }
            }
            // Assert
            Assert.Equal(JsonSerializer.Serialize(originalComponentDraft), JsonSerializer.Serialize(readComponentDraft));
            

            // Clean up
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
