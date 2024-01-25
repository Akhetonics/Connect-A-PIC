﻿using CAP_Contracts;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles;
using CAP_Core.Tiles.Grid;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;

namespace UnitTests
{
    public class ComponentDraftValidatorTests
    {
        public class PathCheckerDummy : IResourcePathChecker
        {
            public bool DoesResourceExist(string godotResourcePath)
            {
                return true;
            }
        }

        [Fact]
        public void ValidateTest()
        {
            var validator = new ComponentDraftValidator(new PathCheckerDummy());
            
            var draft = new ComponentDraft()
            {
                fileFormatVersion = ComponentDraftFileReader.CurrentFileVersion + 1, // trigger ErrorFileVersionNotSupported
                identifier = "", // trigger ErrorIdentifierNotSet
                sceneResPath = "invalid/path", // trigger ErrorSceneResPathNotExist
                widthInTiles = 0, // trigger ErrorWidthInTilesSmaller0
                heightInTiles = 0, // trigger ErrorHeightInTilesSmaller0
                pins = new List<PinDraft>
            {
                new PinDraft { number = 1, partX = -1 }, // trigger ErrorPinPartXSmaller0
                new PinDraft { number = 1, partY = -1 }, // trigger ErrorPinPartYSmaller0 and ErrorPinNumberDuplicated
            },
                overlays = new List<Overlay>
            {
                null, // trigger ErrorOverlayNull
                new Overlay
                {
                    overlayAnimTexturePath = "invalid/path", // trigger ErrorOverlayTexturePathNotExist
                    tileOffsetX = -1, // trigger ErrorOverlayOffsetXSmaller0
                    tileOffsetY = -1, // trigger ErrorOverlayOffsetYSmaller0
                }
            },
                sMatrices = new List<WaveLengthSpecificSMatrix>(){
                    new()
                    {
                        waveLength = StandardWaveLengths.RedNM, 
                        connections = new List<CAP_DataAccess.Components.ComponentDraftMapper.DTOs.Connection>()
                        {
                             new (){ FromPinNr = 999, ToPinNr = 998 } // trigger ErrorFromPinNrInvalid and ErrorToPinNrInvalid
                        }
                    }
                    // the other wavelengths are not defined which should trigger ErrorMatrixNotDefinedForWaveLength
                }
            };


            var result = validator.Validate(draft);

            Assert.False(result.isValid);
            Assert.Contains(ComponentDraftValidator.ErrorFileVersionNotSupported, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorIdentifierNotSet, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorSceneResPathNotExist, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorWidthInTilesSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorHeightInTilesSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorPinPartXSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorPinPartYSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorPinNumberDuplicated, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorOverlayNull, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorOverlayTexturePathNotExist, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorOverlayOffsetXSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorOverlayOffsetYSmaller0, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorFromPinNrInvalid, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorToPinNrInvalid, result.errorMsg);
            Assert.Contains(ComponentDraftValidator.ErrorMatrixNotDefinedForWaveLength, result.errorMsg);
        }

        [Fact]
        public void ValidateDraft_NoErrors_ReturnsValid()
        {
            var validator = new ComponentDraftValidator(new PathCheckerDummy());
            // Arrange
            var draft = new ComponentDraft
            {
                sceneResPath = "res://testHost.exe",
                widthInTiles = 2,
                heightInTiles = 1,
                identifier = "test",
                nazcaFunctionName = "test",
                overlays = new List<Overlay>
                {
                    new Overlay {
                        overlayAnimTexturePath = "res://testHost.exe",
                        tileOffsetX = 0,
                        tileOffsetY = 0,
                    }
                },
                fileFormatVersion = ComponentDraftFileReader.CurrentFileVersion,
                pins = new List<PinDraft>
                {
                    new PinDraft {
                        number = 1,
                        partX = 0,
                        partY = 0,
                        matterType = MatterType.Light,
                        side = RectSide.Left,
                        name = "west0",
                    },
                    new PinDraft {
                        number = 2,
                        partX = 1,
                        partY = 0,
                        matterType = MatterType.Light,
                        side = RectSide.Right,
                        name = "east0",
                    }
                },
                sMatrices = new List<WaveLengthSpecificSMatrix>(){
                    new() {
                        waveLength = StandardWaveLengths.RedNM,
                        connections = new List<CAP_DataAccess.Components.ComponentDraftMapper.DTOs.Connection>()
                        {
                            new ()
                            {
                                FromPinNr = 1,
                                ToPinNr = 2,
                                Formula = "1+PIN0",
                                Imaginary= 0.02f,
                            },
                            new ()
                            {
                                FromPinNr = 2,
                                ToPinNr = 1,
                                Real = 1,
                                Imaginary = 0.02f,
                            },
                        },
                    },
                    new() {
                        waveLength = StandardWaveLengths.GreenNM,
                        connections = new List<CAP_DataAccess.Components.ComponentDraftMapper.DTOs.Connection>()
                        {
                            new ()
                            {
                                FromPinNr = 1,
                                ToPinNr = 2,
                                Formula = "1+PIN0",
                                Imaginary= 0.02f,
                            },
                            new ()
                            {
                                FromPinNr = 2,
                                ToPinNr = 1,
                                Real = 1,
                                Imaginary = 0.02f,
                            },
                        },
                    },
                    new() {
                        waveLength = StandardWaveLengths.BlueNM,
                        connections = new List<CAP_DataAccess.Components.ComponentDraftMapper.DTOs.Connection>()
                        {
                            new ()
                            {
                                FromPinNr = 1,
                                ToPinNr = 2,
                                Formula = "1+PIN0",
                                Imaginary= 0.02f,
                            },
                            new ()
                            {
                                FromPinNr = 2,
                                ToPinNr = 1,
                                Real = 1,
                                Imaginary = 0.02f,
                            },
                        },
                    }
                    // the other wavelengths are not defined which should trigger ErrorMatrixNotDefinedForWaveLength
                }
                
            };

            // Act
            var (isValid, errorMsg) = validator.Validate(draft);

            // Assert
            string currentDir = Directory.GetCurrentDirectory();
            Assert.True(isValid, "The draft should be valid when all conditions are met, but those aren't: " + errorMsg);
            Assert.Equal(string.Empty, errorMsg);
        }
    }
}
