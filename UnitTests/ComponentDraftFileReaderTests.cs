using CAP_Contracts.Logger;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
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
        public class PathCheckerDummy : IDataAccessor
        {
            public bool DoesResourceExist(string godotResourcePath) => true;
            public string ReadAsText(string godotFilePath) => "";
            public bool Write(string filePath, string componentJson) => true;
        }

        public class DummyLogger : ILogger
        {
            public event Action<Log> LogAdded;

            public void PrintErr(string v) { }

            public void PrintInfo(string info)
            {
                throw new NotImplementedException();
            }
        }
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
            var reader = new ComponentDraftFileReader(new PathCheckerDummy());
            reader.Write(tempFilePath, originalComponentDraft);
            bool fileIsUsed = true;
            int counter = 0;
            ComponentDraft readComponentDraft = null;
            while (fileIsUsed)
            {
                try
                {
                    var readResult = reader.TryRead(tempFilePath);
                    if(readResult.error != null)
                    {
                        throw new IOException(readResult.error);
                    }
                    readComponentDraft = readResult.draft;
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
