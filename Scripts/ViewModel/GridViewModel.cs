﻿using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
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
        public ComponentBaseView[,] GridComponentViews { get; private set; }
        public int Width { get => GridComponentViews.GetLength(0); }
        public int Height { get => GridComponentViews.GetLength(1); }
        public Grid Grid { get; set; }
        public GridView GridView { get; set; }
        private GridSMatrixAnalyzer MatrixAnalyzer;
        public int MaxTileCount { get => Width * Height; }
        public GridViewModel(GridView gridview, Grid grid)
        {
            this.GridView = gridview;
            this.Grid = grid;
            //this.GridView.Columns = grid.Width;
            this.GridComponentViews = new ComponentBaseView[grid.Width, grid.Height];
            CreateComponentCommand = new CreateComponentCommand(grid);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            MoveComponentCommand = new MoveComponentCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid);
            CreateEmptyField();
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            ResetTilesAt(x,y,component.WidthInTiles, component.HeightInTiles);
        }
        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int gridX, int gridY)
        {
            Type componentViewType = ComponentViewModelTypeConverter.ToView(component.GetType());
            CreateComponentViewByType(gridX, gridY, component.Rotation90CounterClock, componentViewType, component);
        }
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;
        }

        public void CreateEmptyField()
        {
            foreach (var componentView in GridComponentViews)
            {
                if(componentView?.IsQueuedForDeletion() == false)
                    componentView.QueueFree();
            }
        }
        public static void ShowMultiTileDragPreview(Godot.Vector2 position, ComponentBaseView component, Control dragDropParent)
        {
            component = component.Duplicate();
            component.Visible = true;
            component.Position = new Godot.Vector2(0, 0);
            component.Modulate = new Color(0, 1, 0, 0.5f);
            dragDropParent.SetDragPreview(component);
        }
        public void RegisterComponentView(ComponentBaseView componentView)
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
            for (int x = startX;  x < startX + width; x++)
            {
                for (int y = startY; y < startY +height; y++)
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
        public ComponentBaseView CreateComponentViewByType(int gridx, int gridy, DiscreteRotation rotationCounterClockwise, Type componentViewType, ComponentBase componentModel)
        {
            var ComponentView = ComponentViewFactory.Instance.CreateComponentView(componentViewType);
            ComponentView.Initialize(gridx, gridy, rotationCounterClockwise , this);
            RegisterComponentView(ComponentView);
            GridView.DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
            return ComponentView;
        }

        public Dictionary<Guid, Complex> GetLightVector(LightColor color)
        {
            MatrixAnalyzer ??= new GridSMatrixAnalyzer(this.Grid);
            return MatrixAnalyzer.CalculateLightPropagation(color);
        }

        public void ShowLightPropagation(Dictionary<Guid, Complex> lightVectorRed, Dictionary<Guid, Complex> lightVectorGreen, Dictionary<Guid, Complex> lightVectorBlue)
        {
            // go through the whole grid and send all 
            AssignLightToComponentViews(lightVectorRed, LightColor.Red);
            AssignLightToComponentViews(lightVectorGreen, LightColor.Green);
            AssignLightToComponentViews(lightVectorBlue, LightColor.Blue);
        }

        private void AssignLightToComponentViews(Dictionary<Guid, Complex> lightVector, LightColor color)
        {
            List<ComponentBase> components = Grid.GetAllComponents();
            foreach( var componentModel in components)
            {
                List<LightAtPin> lightAtPins = new();

                for(int offsetX = 0; offsetX < componentModel.WidthInTiles; offsetX++)
                {
                    for( int offsetY = 0; offsetY < componentModel.HeightInTiles; offsetY++)
                    {
                        var part = componentModel.GetPartAt(offsetX, offsetY);
                        foreach (var side in Enum.GetValues(typeof(RectSide)).OfType<RectSide>())
                        {
                            var pin = part.GetPinAt(side);
                            if (pin == null) continue;
                            var lightIntensityIn = lightVector[pin.IDInFlow].Real;
                            var lightPhaseIn = lightVector[pin.IDInFlow].Phase;
                            var lightIntensityOut = lightVector[pin.IDOutFlow].Real;
                            var lightPhaseOut = lightVector[pin.IDOutFlow].Phase;

                            var lightFlow = new LightAtPin(
                                offsetX,
                                offsetY,
                                side,
                                color,
                                new Complex(lightIntensityIn, lightPhaseIn),
                                new Complex(lightIntensityOut, lightPhaseOut)
                                );
                            lightAtPins.Add(lightFlow);
                        }
                    }
                }
                GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile].DisplayLightVector(lightAtPins);
            }
            
        }

        public void HideLightPropagation()
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    GridComponentViews[x, y]?.HideLightVector();
                }
            }
        }
    }
}
