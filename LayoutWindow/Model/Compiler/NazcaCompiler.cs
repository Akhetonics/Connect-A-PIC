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
        private List<ComponentBase> AlreadyProcessedComponents;
        private StringBuilder ExportAllConnectedTiles(Tile parent, Tile child)
        {
            var nazcaString = new StringBuilder();
            nazcaString.Append(child.ExportToNazca(parent));
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

        private void ConnectComponentsAtInputsViaPin(StringBuilder NazcaCode)
        {
            foreach (ExternalPort port in grid.ExternalPorts)
            {
                if (port is not StandardInput input) continue;
                var x = 0;
                var y = input.TilePositionY;
                if (!grid.IsInGrid(x, y, 1, 1)) continue;
                var firstConnectedTile = grid.Tiles[x, y];
                if (firstConnectedTile.Component == null) continue;
                StartConnectingAtInput(NazcaCode, input, firstConnectedTile);
            }
            // go through rest of components, start with one that is not being added to the NazcaCode yet
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var comp = grid.Tiles[x, y].Component;
                    if (comp == null) continue;
                    if (AlreadyProcessedComponents.Contains(comp)) continue;
                    StartConnectingAtTile(NazcaCode, grid.Tiles[x, y]);
                }
            }
        }
        
        private void StartConnectingAtTile(StringBuilder NazcaCode, Tile currentTile)
        {
            NazcaCode.Append(currentTile.ExportToNazcaAbsolutePosition());
            AlreadyProcessedComponents.Add(currentTile.Component);
            ExportAllNeighbours(NazcaCode, currentTile);
        }

        private void ExportAllNeighbours(StringBuilder NazcaCode, Tile currentTile)
        {
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
            NazcaCode.Append(firstConnectedTile.ExportToNazcaExtended(new IntVector(-1, input.TilePositionY), StandardInputCellName, input.PinName));
            AlreadyProcessedComponents.Add(firstConnectedTile.Component);
            ExportAllNeighbours(NazcaCode, firstConnectedTile);
        }
    }
}