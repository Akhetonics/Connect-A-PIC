﻿using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel
    {
        public ICommand CreateComponentCommand { get; set; }
        public ICommand MoveComponentCommand { get; set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        public ICommand ExportToNazcaCommand { get; set; }
        public ComponentView[,] GridComponentViews { get; private set; }
        public int Width { get => GridComponentViews.GetLength(0); }
        public int Height { get => GridComponentViews.GetLength(1); }
        public Grid Grid { get; set; }
        public GridView GridView { get; set; }
        public GridSMatrixAnalyzer MatrixAnalyzer { get; private set; }
        public int MaxTileCount { get => Width * Height; }
        public GridViewModel(GridView gridview, Grid grid)
        {
            this.GridView = gridview;
            this.Grid = grid;
            //this.GridView.Columns = grid.Width;
            this.GridComponentViews = new ComponentView[grid.Width, grid.Height];
            CreateComponentCommand = new CreateComponentCommand(grid);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            MoveComponentCommand = new MoveComponentCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid);
            CreateEmptyField();
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        private void Grid_OnComponentRemoved(Component component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
            if (GridView.lightPropagationIsPressed)
            {
                HideLightPropagation();
                ShowLightPropagation();
            }
        }
        private void Grid_OnComponentPlacedOnTile(Component component, int gridX, int gridY)
        {
            CreateComponentView(gridX, gridY, component.Rotation90CounterClock, component.TypeNumber);
            if (GridView.lightPropagationIsPressed)
            {
                HideLightPropagation();
                ShowLightPropagation();
            }
        }
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;
        }

        public void CreateEmptyField()
        {
            foreach (var componentView in GridComponentViews)
            {
                if (componentView?.IsQueuedForDeletion() == false)
                    componentView.QueueFree();
            }
        }

        public void RegisterComponentViewInGridView(ComponentView componentView)
        {
            for (int x = componentView.GridX; x < componentView.GridX + componentView.WidthInTiles; x++)
            {
                for (int y = componentView.GridY; y < componentView.GridY + componentView.HeightInTiles; y++)
                {
                    if (!IsInGrid(x, y, 1, 1)) continue;
                    GridComponentViews[x, y] = componentView;
                }
            }
        }
        public void ResetTilesAt(int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    if (!IsInGrid(x, y, 1, 1)) continue;
                    var oldComponent = GridComponentViews[x, y];
                    if (oldComponent == null) continue;
                    if (!oldComponent.IsQueuedForDeletion())
                    {
                        oldComponent.QueueFree();
                    }
                    GridComponentViews[x, y] = null;
                }
            }
        }
        public ComponentView CreateComponentView(int gridx, int gridy, DiscreteRotation rotationCounterClockwise, int componentTypeNumber)
        {
            var ComponentView = ComponentViewFactory.Instance.CreateComponentView(componentTypeNumber);
            ComponentView.RegisterInGrid(gridx, gridy, rotationCounterClockwise, this);
            RegisterComponentViewInGridView(ComponentView);
            GridView.DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
            return ComponentView;
        }

        public Dictionary<Guid, Complex> GetLightVector(LightColor color)
        {
            MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid);
            return MatrixAnalyzer.CalculateLightPropagation(color);
        }

        public void ShowLightPropagation()
        {
            var lightVectorRed = GetLightVector(LightColor.Red);
            var lightVectorGreen = GetLightVector(LightColor.Green);
            var lightVectorBlue = GetLightVector(LightColor.Blue);
            // go through the whole grid and send all 
            AssignLightToComponentViews(lightVectorRed, LightColor.Red);
            AssignLightToComponentViews(lightVectorGreen, LightColor.Green);
            AssignLightToComponentViews(lightVectorBlue, LightColor.Blue);
        }

        private void AssignLightToComponentViews(Dictionary<Guid, Complex> lightVector, LightColor color)
        {
            List<Component> components = Grid.GetAllComponents();
            foreach (var componentModel in components)
            {
                try
                {
                    List<LightAtPin> lightAtPins = CalculateLightAtPins(lightVector, color, componentModel);
                    GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile].DisplayLightVector(lightAtPins);
                }
                catch (Exception ex)
                {
                    CustomLogger.PrintEx(ex);
                }

            }
        }

        public static List<LightAtPin> CalculateLightAtPins(Dictionary<Guid, Complex> lightVector, LightColor color, Component componentModel)
        {
            List<LightAtPin> lightAtPins = new();

            for (int offsetX = 0; offsetX < componentModel.WidthInTiles; offsetX++)
            {
                for (int offsetY = 0; offsetY < componentModel.HeightInTiles; offsetY++)
                {
                    var part = componentModel.GetPartAt(offsetX, offsetY);
                    foreach (var localSide in Enum.GetValues(typeof(RectSide)).OfType<RectSide>())
                    {
                        var pin = part.GetPinAt(localSide);
                        if (pin == null) continue;
                        var lightFlow = new LightAtPin(
                            offsetX,
                            offsetY,
                            localSide,
                            color,
                            lightVector.TryGetVal(pin.IDInFlow),
                            lightVector.TryGetVal(pin.IDOutFlow)
                            );
                        lightAtPins.Add(lightFlow);
                    }
                }
            }

            return lightAtPins;
        }

        public void HideLightPropagation()
        {
            try
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    for (int y = 0; y < Grid.Height; y++)
                    {
                        GridComponentViews[x, y]?.HideLightVector();
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.PrintEx(ex);
            }

        }
    }
}