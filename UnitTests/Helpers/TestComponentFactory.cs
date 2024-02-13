using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using CAP_DataAccess.Components.ComponentDraftMapper;
using System.Numerics;
using System.Reflection;
using System.Resources;

namespace UnitTests
{
    public class TestComponentFactory
    {
        public static string StraightWGJson => GetResourceContent("StraightWG");
        public static string DirectionalCouplerJSON => GetResourceContent("DirectionalCouplerDraft");
        public static string GetResourceContent(string resourcePath)
        {
            var resourceManager = new ResourceManager("UnitTests.Properties.Resources", Assembly.GetExecutingAssembly());
            var content = resourceManager.GetString(resourcePath);
            return content;
        }
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
            var matrixRed = new SMatrix(allPins, new());
            // set the connections
            matrixRed.SetValues(new(){
                { (leftIn, rightOut), 1 },
                { (rightIn, leftOut), 1 },
            });
            var connections = new Dictionary<int, SMatrix>
            {
                { StandardWaveLengths.RedNM, matrixRed},
                { StandardWaveLengths.GreenNM, matrixRed},
                { StandardWaveLengths.BlueNM, matrixRed},
            };

            return new Component(connections, new(), "placeCell_StraightWG", "", parts, 0, "Straight", DiscreteRotation.R0);
        }

        public static Component CreateDirectionalCoupler()
        {
            int widthInTiles = 2;
            int heightInTiles = 2;

            Part[,] parts = new Part[widthInTiles, heightInTiles];


            parts[0, 0] = new Part(new List<Pin>() { new Pin("west0", 0, MatterType.Light, RectSide.Left) });
            parts[1, 0] = new Part(new List<Pin>() { new Pin("east0", 1, MatterType.Light, RectSide.Right) });
            parts[1, 1] = new Part(new List<Pin>() { new Pin("east1", 2, MatterType.Light, RectSide.Right) });
            parts[0, 1] = new Part(new List<Pin>() { new Pin("west1", 3, MatterType.Light, RectSide.Left) });

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
            var matrixRed = new SMatrix(allPins, new());
            // set the connections
            matrixRed.SetValues(new(){
                { (leftUpIn, rightUpOut), Math.Sqrt(0.5f) },
                { (leftUpIn, rightDownOut), Math.Sqrt(0.5f) },
                { (leftDownIn, rightUpOut), Math.Sqrt(0.5f) },
                { (leftDownIn, rightDownOut), Math.Sqrt(0.5f) },
                { (rightUpIn, leftUpOut), Math.Sqrt(0.5f) },
                { (rightUpIn, leftDownOut), Math.Sqrt(0.5f) },
                { (rightDownIn, leftUpOut), Math.Sqrt(0.5f) },
                { (rightDownIn, leftDownOut), Math.Sqrt(0.5f) },

            });
            var connections = new Dictionary<int, SMatrix>
            {
                {StandardWaveLengths.RedNM, matrixRed},
                {StandardWaveLengths.GreenNM, matrixRed},
                {StandardWaveLengths.BlueNM, matrixRed},
            };
            return new Component(connections, new(), "placeCell_DirectionalCoupler", "", parts, 0, "DirectionalCoupler", DiscreteRotation.R0);
        }

        public static Component CreateComponent(string componentJson)
        {
            var dummyJsonDataAccessor = new DummyDataAccessor(componentJson);
            var componentDraft = new ComponentDraftFileReader(dummyJsonDataAccessor).TryReadJson("").draft;
            if (componentDraft == null)
            {
                throw new Exception("JSON could not be parsed");
            }
            var drafts = new List<ComponentDraft>() { componentDraft };
            var validator = new ComponentDraftValidator(dummyJsonDataAccessor);
            string draftErrors = "";
            foreach (var item in drafts.Select(d => validator.Validate(d)).ToList())
            {
                draftErrors += item.errorMsg;
            };
            if (String.IsNullOrEmpty(draftErrors) == false)
                throw new Exception(draftErrors);

            var draftConverter = new ComponentDraftConverter(new Logger());
            var componentDrafts = draftConverter.ToComponentModels(drafts);
            return componentDrafts.First();
        }

    }
}
