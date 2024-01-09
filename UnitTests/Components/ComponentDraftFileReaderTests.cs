﻿using CAP_Contracts.Logger;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
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
            // Arrange
            var tempFilePath = Path.GetTempFileName();
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
                        matterType = CAP_Core.Components.MatterType.Light,
                        name = "west0",
                        number = 1,
                        partX = 0,
                        partY = 0,
                        side = RectSide.Left,
                    }
                },
                connections = new List<Connection>
                {
                    new ()
                    {
                        fromPinNr = 1,
                        toPinNr = 1,
                        magnitude = 1,
                        wireLengthNM = 0.02f,
                    }
                },
                overlays = new List<Overlay>
                {
                    new ()
                    {
                        overlayAnimTexturePath = "res://Scenes/Components/DirectionalCoupler.tscn",
                        tileOffsetX = 0,
                        tileOffsetY = 0,
                    }
                },
                deltaLength = 1,
            };

            // Act
            var reader = new ComponentDraftFileReader(new FileDataAccessor());
            await reader.Write(tempFilePath, originalComponentDraft);
            bool fileIsUsed = true;
            int counter = 0;
            ComponentDraft readComponentDraft = null;
            while (fileIsUsed)
            {
                try
                {
                    var readResult = reader.TryReadJson(tempFilePath);
                    if(readResult.error == null)
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
