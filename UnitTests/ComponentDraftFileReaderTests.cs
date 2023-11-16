using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests
{
    public class ComponentDraftFileReaderTests
    {
        [Fact]
        public async Task TestReadWrite()
        {
            // Arrange
            var tempFilePath = Path.Combine(Path.GetTempPath(), "tempComponentDraft.json");
            var originalComponentDraft = new ComponentDraft
            {
                fileFormatVersion = 1,
                identifier = "test",
                nazcaFunctionName = "test",
                nazcaFunctionParameters = "",
                sceneResPath = "res://Scenes/Components/DirectionalCoupler.tscn",
                widthInTiles = 1,
                heightInTiles =1,
                pins = new List<PinDraft>
                {
                    new PinDraft()
                    {
                        matterType = MatterType.Light,
                        name = "west0",
                        number = 1,
                        partX = 0,
                        partY = 0,
                        side = RectSide.Left,
                    }
                },
                connections = new List<Connection>
                {
                    new Connection()
                    {
                        fromPinNr = 1,
                        toPinNr = 1,
                        magnitude = 1,
                        wireLengthNM = 0.02f,
                    }
                },
                overlays = new List<Overlay>
                {
                    new Overlay()
                    {
                        overlayAnimTexturePath = "res://Scenes/Components/DirectionalCoupler.tscn",
                        tileOffsetX = 0,
                        tileOffsetY = 0,
                    }
                },
                deltaLength = 1,
            };

            // Act
            ComponentDraftFileReader.Write(tempFilePath, originalComponentDraft);
            bool fileIsUsed = true;
            int counter = 0;
            ComponentDraft readComponentDraft = null;
            while (fileIsUsed)
            {
                try
                {
                    readComponentDraft = ComponentDraftFileReader.TryRead(tempFilePath);
                    fileIsUsed = false;
                } catch (IOException)
                {
                    await Task.Delay(10);
                    counter++;
                    if (counter > 100)
                        throw;
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
