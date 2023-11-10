using CAP_Core.Component.ComponentDraftMapper;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;

namespace UnitTests
{
    public class ComponentDraftValidatorTests
    {
        [Fact]
        public void ValidateTest()
        {
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
                connections = new List<Connection>
            {
                new Connection { fromPinNr = 999, toPinNr = 998 } // trigger ErrorFromPinNrInvalid and ErrorToPinNrInvalid
            }
            };
            var result = ComponentDraftValidator.Validate(draft);

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
        }

        [Fact]
        public void ValidateDraft_NoErrors_ReturnsValid()
        {
            // Arrange
            var draft = new ComponentDraft
            {
                sceneResPath = "res://testhost.exe",
                widthInTiles = 2,
                heightInTiles = 1,
                identifier = "test",
                nazcaFunctionName = "test",
                overlays = new List<Overlay>
                {
                    new Overlay {
                        overlayAnimTexturePath = "res://testhost.exe",
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
                        matterType = CAP_Core.Component.ComponentHelpers.MatterType.Light,
                        side = RectSide.Left,
                        name = "west0",
                    },
                    new PinDraft {
                        number = 2,
                        partX = 1,
                        partY = 0,
                        matterType = CAP_Core.Component.ComponentHelpers.MatterType.Light,
                        side = RectSide.Right,
                        name = "east0",
                    }
                },
                connections = new List<Connection>()
                {
                    new Connection()
                    {
                        fromPinNr = 1,
                        toPinNr = 2,
                        magnitude = 1,
                        wireLengthNM = 0.02f,
                    },
                    new Connection()
                    {
                        fromPinNr = 2,
                        toPinNr = 1,
                        magnitude = 1,
                        wireLengthNM = 0.02f,
                    },
                },
            };

            // Act
            var (isValid, errorMsg) = ComponentDraftValidator.Validate(draft);

            // Assert
            string currentDir = Directory.GetCurrentDirectory();
            Assert.True(isValid, "The draft should be valid when all conditions are met, but those aren't: " + errorMsg);
            Assert.Equal(string.Empty, errorMsg);
        }
    }
}
