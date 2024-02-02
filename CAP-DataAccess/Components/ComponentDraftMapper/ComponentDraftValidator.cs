using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CAP_Contracts;
using CAP_Core.Components.ComponentHelpers;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using MathNet.Numerics;

namespace CAP_DataAccess.Components.ComponentDraftMapper
{
    public class ComponentDraftValidator
    {
        public static readonly string ErrorOverlayCountIsNull = "Err_001";
        public static readonly string ErrorFileVersionNotSupported = "Err_002";
        public static readonly string ErrorNoPinsDefined = "Err_003";
        public static readonly string ErrorSceneResPathNotExist = "Err_004";
        public static readonly string ErrorWidthInTilesSmaller0 = "Err_005";
        public static readonly string ErrorHeightInTilesSmaller0 = "Err_006";
        public static readonly string ErrorIdentifierNotSet = "Err_007";
        public static readonly string ErrorOverlayNull = "Err_008";
        public static readonly string ErrorOverlayTexturePathNotExist = "Err_009";
        public static readonly string ErrorOverlayOffsetXSmaller0 = "Err_010";
        public static readonly string ErrorOverlayOffsetYSmaller0 = "Err_011";
        public static readonly string ErrorPinPartXSmaller0 = "Err_012";
        public static readonly string ErrorPinPartYSmaller0 = "Err_013";
        public static readonly string ErrorPinNumberDuplicated = "Err_014";
        public static readonly string ErrorFromPinNrInvalid = "Err_015";
        public static readonly string ErrorToPinNrInvalid = "Err_016";
        public static readonly string ErrorPinPartYBiggerMaxHeight = "Err_017";
        public static readonly string ErrorPinPartXBiggerMaxHeight = "Err_018";
        public static readonly string ErrorMatrixNotDefinedForWaveLength = "Err_019";

        public IResourcePathChecker ResourcePathChecker { get; }

        public ComponentDraftValidator(IResourcePathChecker resourcePathChecker)
        {
            this.ResourcePathChecker = resourcePathChecker;
        }
        public bool ResourceExists(string godotPath)
        {
            if (!godotPath.StartsWith("res://"))
            {
                throw new ArgumentException("Path has to start with 'res://'.", nameof(godotPath));
            }
            return ResourcePathChecker.DoesResourceExist(godotPath);
        }

        public (bool isValid, string errorMsg) Validate(ComponentDraft draft)
        {
            string errorMsg = "";
            if(draft == null)
            {
                throw new Exception("Draft cannot be null - there might have been some error while parsing the json of the component draft");
            }
            if (draft.Overlays == null || draft.Overlays.Count == 0)
            {
                errorMsg += ErrorOverlayCountIsNull + $" {nameof(draft.Overlays)}.count is 0 - no overlays are defined\n";
            }
            if (draft.FileFormatVersion > ComponentDraftFileReader.CurrentFileVersion)
            {
                errorMsg += ErrorFileVersionNotSupported + $" {nameof(draft.FileFormatVersion)} is higher than what this software can handle. The max Version readable is {ComponentDraftFileReader.CurrentFileVersion}\n";
            }
            if (draft.Pins == null ||draft.Pins.Count == 0)
            {
                errorMsg += ErrorNoPinsDefined + $" There are no {nameof(draft.Pins)} defined at all. At least 1 pin should be defined\n";
            }
            if (string.IsNullOrWhiteSpace(draft.SceneResPath))
            {
                errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.SceneResPath)} is not set\n";
            }
            try
            {
                if (!ResourceExists(draft.SceneResPath))
                {
                    errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.SceneResPath)} does not exist on disk\n";
                }
            }
            catch (Exception ex)
            {
                errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.SceneResPath)} - {ex.Message}\n";
            }

            if (draft.WidthInTiles == 0)
            {
                errorMsg += ErrorWidthInTilesSmaller0 + $" {nameof(draft.WidthInTiles)} has to be greater than 0\n";
            }
            if (draft.HeightInTiles == 0)
            {
                errorMsg += ErrorHeightInTilesSmaller0 + $" {nameof(draft.HeightInTiles)} has to be greater than 0\n";
            }
            if (string.IsNullOrWhiteSpace(draft.Identifier))
            {
                errorMsg += ErrorIdentifierNotSet + $" {nameof(draft.Identifier)} has to be defined\n";
            }
            if(draft?.Overlays != null)
            {
                foreach (var overlay in draft.Overlays)
                {
                    errorMsg += ValidateOverlay(overlay);
                }
            }
            
            errorMsg += ValidatePinNumbersAreUnique(draft.Pins);
            if(draft?.Pins != null)
            {
                foreach (var pin in draft.Pins)
                {
                    errorMsg += ValidatePin(pin, draft.WidthInTiles, draft.HeightInTiles);
                }
            }
            
            if(draft?.SMatrices == null)
            {
                errorMsg += $"{ErrorMatrixNotDefinedForWaveLength} sMatrix is not defined for any WaveLength\n";
            } else
            {
                foreach (var connection in draft.SMatrices)
                {
                    errorMsg += ValidateConnection(draft.Pins, draft.SMatrices);
                }
            }
            

            bool success = true;
            if (errorMsg.Length > 0)
            {
                errorMsg += $"in ComponentDraft {draft.Identifier}\n";
                success = false;
            }
            return (success, errorMsg);
        }

        private string ValidateOverlay(Overlay overlay)
        {
            string errorMsg = "";
            if (overlay == null)
            {
                errorMsg += ErrorOverlayNull + " overlay cannot be null\n";
                return errorMsg;
            }
            try
            {
                if (!ResourceExists(overlay.overlayAnimTexturePath))
                {
                    errorMsg += ErrorOverlayTexturePathNotExist + $" {nameof(overlay.overlayAnimTexturePath) + " " + overlay.overlayAnimTexturePath} does not exist on disk\n";
                }
            }
            catch (Exception ex)
            {
                errorMsg += ErrorOverlayTexturePathNotExist + $" {nameof(overlay.overlayAnimTexturePath) + " '" + overlay?.overlayAnimTexturePath + "'" + ex.Message}' \n";
            }

            if (overlay.tileOffsetX < 0)
            {
                errorMsg += ErrorOverlayOffsetXSmaller0 + $" {nameof(overlay.tileOffsetX)} cannot be < 0\n";
            }
            if (overlay.tileOffsetY < 0)
            {
                errorMsg += ErrorOverlayOffsetYSmaller0 + $" {nameof(overlay.tileOffsetY)} cannot be < 0\n";
            }
            return errorMsg;
        }

        private static string ValidatePin(PinDraft pin, int widthInTiles, int heightInTiles)
        {
            string errorMsg = "";
            if (pin.PartX < 0)
            {
                errorMsg += ErrorPinPartXSmaller0 + $" {nameof(pin.PartX)} has to be >= 0\n";
            }
            if (pin.PartX > widthInTiles)
            {
                errorMsg += ErrorPinPartXBiggerMaxHeight + $" {nameof(pin.PartX)} has to be < widthInTiles \n";
            }
            if (pin.PartY < 0)
            {
                errorMsg += ErrorPinPartYSmaller0 + $" {nameof(pin.PartY)} has to be >= 0\n";
            }
            if (pin.PartY > heightInTiles)
            {
                errorMsg += ErrorPinPartYBiggerMaxHeight + $" {nameof(pin.PartY)} has to be < heightInTiles \n";
            }
            return errorMsg;
        }
        private static string ValidatePinNumbersAreUnique(List<PinDraft> pins)
        {
            var duplicateNumbers = pins
                .GroupBy(p => p.Number)
                .Where(p => p.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateNumbers.Any())
            {
                string numbers = string.Join(',', duplicateNumbers);
                return ErrorPinNumberDuplicated + $" Each Pin must have a unique {nameof(PinDraft.Number)}, but '{numbers}' had been used twice\n";
            }
            return "";
        }

        private static string ValidateConnection(List<PinDraft> pins, List<WaveLengthSpecificSMatrix> matrixDrafts)
        {
            string errorMsg = "";
            // test if the SMatrices are defined for all given standard wavelengths
            var definedWaveLengths = matrixDrafts.Select(m => m.WaveLength).ToList(); 
            // we use Reflection to get all properties as this class is used as an enum
            foreach (PropertyInfo prop in typeof(StandardWaveLengths).GetProperties(BindingFlags.Public | BindingFlags.Static)) 
            {
                int waveLength = (int)prop.GetValue(null);
                if (!definedWaveLengths.Contains(waveLength))
                {
                    errorMsg += ErrorMatrixNotDefinedForWaveLength + $" SMatrix is not defined for the standard-waveLength: '{waveLength} nanometer' \n";
                }
            }

            // test if all pins exist that are being used in the connections of the SMatrices
            foreach(var matrix in matrixDrafts)
            {
                foreach(var connection in matrix.Connections)
                {
                    var allPinNumbers = pins.Select(p => p.Number).ToHashSet();
                    if (!allPinNumbers.Contains(connection.FromPinNr))
                    {
                        errorMsg += ErrorFromPinNrInvalid + $"The number '{nameof(Connection.FromPinNr)}' is not defined in the list of Pins\n";
                    }
                    if (!allPinNumbers.Contains(connection.ToPinNr))
                    {
                        errorMsg += ErrorToPinNrInvalid + $"The number '{nameof(Connection.ToPinNr)}' is not defined in the list of Pins\n";
                    }
                }
            }
            
            return errorMsg;
        }


    }

}
