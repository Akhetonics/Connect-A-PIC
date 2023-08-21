using ConnectAPIC.LayoutWindow.Model.Compiler;
using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using MathNet.Numerics.Distributions;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using Tiles;

namespace ConnectAPIC.Scenes.Compiler
{
    public class NazcaCompiler : ICompiler
    {
        private Grid grid;
        public const string PDKName = "CAPICPDK";
        public const string StandardInputCellName = "grating";
        public List<ComponentBase> AlreadyProcessedComponents;
        private StringBuilder ExportAllConnectedTiles(Tile parent, Tile child)
        {
            var nazcaString = new StringBuilder();
            nazcaString.Append(child.ExportToNazca(parent, child.Component.NazcaFunctionParameters));
            AlreadyProcessedComponents.Add(child.Component);
            var neighbours = grid.GetConnectedNeighboursOfComponent(child.Component);
            neighbours = neighbours.Where(n => !AlreadyProcessedComponents.Contains(n.Component)).ToList();
            foreach (Tile childsNeighbourTile in neighbours)
            {
                nazcaString.Append(ExportAllConnectedTiles(child, childsNeighbourTile));
            }
            return nazcaString;
        }
        public string Compile(Grid grid)
        {
            this.grid = grid;
            AlreadyProcessedComponents = new List<ComponentBase>();
            StringBuilder NazcaCode = new();
            NazcaCode.Append(PythonResources.CreateHeader(PDKName, StandardInputCellName));
            // start at all the three intputTiles.
            ConnectComponentsAtInputsViaPin( NazcaCode);
            NazcaCode.Append(PythonResources.CreateFooter());
            return NazcaCode.ToString();
        }

        private void ConnectComponentsAtInputsViaPin( StringBuilder NazcaCode)
        {
            foreach (ExternalPort port in grid.ExternalPorts)
            {
                if (port is not StandardInput input) continue;
                var x = 0;
                var y = input.TilePositionY;
                if (!grid.IsInGrid(x, y, 1, 1)) continue;
                var firstConnectedTile = grid.Tiles[x, y];
                if (firstConnectedTile.Component == null) continue;
                StartConnectingAtInput( NazcaCode, input, firstConnectedTile);
            }
        }
        private void StartConnectingAtTile(StringBuilder NazcaCode, string ParentPinName, int parentX, int parentY, string CellName, Tile currentTile)
        {
            NazcaCode.Append(currentTile.ExportToNazcaExtended(new IntVector(parentX, parentY), CellName, ParentPinName, currentTile.Component.NazcaFunctionParameters));
            AlreadyProcessedComponents.Add(currentTile.Component);
            var neighbours = grid.GetConnectedNeighboursOfComponent(currentTile.Component);
            if (neighbours != null)
            {
                foreach (Tile neighbour in neighbours)
                {
                    NazcaCode.Append(ExportAllConnectedTiles(currentTile, neighbour));
                }
            }
        }
        private void StartConnectingAtInput( StringBuilder NazcaCode, StandardInput input, Tile firstConnectedTile)
        {
            StartConnectingAtTile(NazcaCode, input.PinName, -1 , input.TilePositionY, StandardInputCellName, firstConnectedTile);

        }
    }
}