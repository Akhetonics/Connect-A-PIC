using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAP_Core.Component.ComponentDraftMapper
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

        public static bool ResourceExists(string godotPath)
        {
            if (!godotPath.StartsWith("res://"))
            {
                throw new ArgumentException("Path has to start with 'res://'.", nameof(godotPath));
            }

            var windowsPath = godotPath.Replace("res://", "").Replace("/", "\\");

            return File.Exists(windowsPath);
        }

        public static (bool isValid, string errorMsg) Validate(ComponentDraft draft)
        {
            string errorMsg = "";
            if (draft.overlays.Count == 0)
            {
                errorMsg += ErrorOverlayCountIsNull + $" {nameof(draft.overlays)}.count is null\n";
            }
            if (draft.fileFormatVersion > ComponentDraftFileReader.CurrentFileVersion)
            {
                errorMsg += ErrorFileVersionNotSupported + $" {nameof(draft.fileFormatVersion)} is higher than what this software can handle. The max Version readable is {ComponentDraftFileReader.CurrentFileVersion}\n";
            }
            if (draft.pins.Count == 0)
            {
                errorMsg += ErrorNoPinsDefined + $" There are no {nameof(draft.pins)} defined at all. At least 1 pin should be defined\n";
            }
            if (String.IsNullOrWhiteSpace(draft.sceneResPath))
            {
                errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.sceneResPath)} is not set\n";
            }
            try
            {
                if (!ResourceExists(draft.sceneResPath))
                {
                    errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.sceneResPath)} does not exist on disk\n";
                }
            } catch (Exception ex)
            {
                errorMsg += ErrorSceneResPathNotExist + $" {nameof(draft.sceneResPath)} - {ex.Message}\n";
            }
            
            if (draft.widthInTiles == 0)
            {
                errorMsg += ErrorWidthInTilesSmaller0 + $" {nameof(draft.widthInTiles)} has to be greater than 0\n";
            }
            if (draft.heightInTiles == 0)
            {
                errorMsg += ErrorHeightInTilesSmaller0 + $" {nameof(draft.heightInTiles)} has to be greater than 0\n";
            }
            if (String.IsNullOrWhiteSpace(draft.identifier))
            {
                errorMsg += ErrorIdentifierNotSet + $" {nameof(draft.identifier)} has to be defined\n";
            }
            foreach (var overlay in draft.overlays)
            {
                errorMsg += ValidateOverlay(overlay);
            }
            errorMsg += ValidatePinNumbersAreUnique(draft.pins);
            foreach (var pin in draft.pins)
            {
                errorMsg += ValidatePin(pin, draft.widthInTiles, draft.heightInTiles);
            }
            foreach (var connection in draft.connections)
            {
                errorMsg += ValidateConnection(draft.pins, connection);
            }

            bool success = true;
            if (errorMsg.Length > 0)
            {
                errorMsg += $"in ComponentDraft {draft.GetType().Name}\n";
                success = false;
            }
            return (success, errorMsg);
        }

        private static string ValidateOverlay(Overlay overlay)
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
            }catch (Exception ex)
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
        // create a function that calculates the phase and the magnitude of the signal at a given time t and a given frequency f
        // the function should return a tuple of (phase, magnitude)
        // the function should be called "calculatePhaseAndMagnitude"
        // the function should take 2 parameters: t and f

        private static string ValidatePin(PinDraft pin, int widthInTiles, int heightInTiles)
        {
            string errorMsg = "";
            if (pin.partX < 0)
            {
                errorMsg += ErrorPinPartXSmaller0 + $" {nameof(pin.partX)} has to be >= 0\n";
            }
            if (pin.partX > widthInTiles)
            {
                errorMsg += ErrorPinPartXBiggerMaxHeight + $" {nameof(pin.partX)} has to be < widthInTiles \n";
            }
            if (pin.partY < 0)
            {
                errorMsg += ErrorPinPartYSmaller0 + $" {nameof(pin.partY)} has to be >= 0\n";
            }
            if (pin.partY > heightInTiles)
            {
                errorMsg += ErrorPinPartYBiggerMaxHeight + $" {nameof(pin.partY)} has to be < heightInTiles \n";
            }
            return errorMsg;
        }
        private static string ValidatePinNumbersAreUnique(List<PinDraft> pins)
        {
            var duplicateNumbers = pins
                .GroupBy(p => p.number)
                .Where(p => p.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateNumbers.Any())
            {
                string numbers = String.Join(',', duplicateNumbers);
                return ErrorPinNumberDuplicated + $" Each Pin must have a unique {nameof(PinDraft.number)}, but '{numbers}' had been used twice\n";
            }
            return "";
        }
        private static string ValidateConnection(List<PinDraft> pins, Connection connection)
        {
            string errorMsg = "";
            // the pinIDs on the connections should exist
            var allPinNumbers = pins.Select(p => p.number).ToHashSet();
            if (!allPinNumbers.Contains(connection.fromPinNr))
            {
                errorMsg += ErrorFromPinNrInvalid + $" The number '{nameof(Connection.fromPinNr)}' is not defined in the list of Pins\n";
            }
            if (!allPinNumbers.Contains(connection.toPinNr))
            {
                errorMsg += ErrorToPinNrInvalid + $"the number '{nameof(Connection.toPinNr)}' is not defined in the list of Pins\n";
            }
            return errorMsg;
        }


    }

}
