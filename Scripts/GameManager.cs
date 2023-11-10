using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace ConnectAPic.LayoutWindow
{
	public partial class GameManager : Node
	{
		[Export] public NodePath GridViewPath { get; set; }
		[Export] public int FieldWidth { get; set; } = 24;

		[Export] public int FieldHeight { get; set; } = 12;
		[Export] public TextureRect ExternalOutputTemplate { get; set; }
		[Export] public TextureRect ExternalInputRedTemplate { get; set; }
		[Export] public TextureRect ExternalInputGreenTemplate { get; set; }
		[Export] public TextureRect ExternalInputBlueTemplate { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public static int TileBorderLeftDown { get; private set; } = 2;
		public GridView GridView { get; set; }
		public Grid Grid { get; set; }
		public static GameManager instance;
		public GridViewModel GridViewModel { get; private set; }
		public static GameManager Instance
		{
			get { return instance; }
		}

		public override void _Ready()
		{
			if (instance == null)
			{
				instance = this;
				GridView = GetNode<GridView>(GridViewPath);
				Grid = new Grid(FieldWidth, FieldHeight);
				GridViewModel = new GridViewModel(GridView, Grid);
				GridView.Initialize(GridViewModel);
				InitializeExternalPortViews(Grid.ExternalPorts);
				CallDeferred(nameof(InitializeAllComponentDrafts));
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}
		
		private void InitializeAllComponentDrafts()
		{
			ComponentImporter.ImportAllPCKComponents(ComponentImporter.ComponentFolderPath);
			var componentDrafts = ComponentImporter.ImportAllJsonComponents();
			ComponentViewFactory.Instance.InitializeComponentDrafts(componentDrafts);
			
			
			var modelComponents = new List<Component>();
			int typeNumber = 0;
			foreach ( var draft in componentDrafts)
			{
				// convert PinDrafts to Model Pins
				Dictionary<(int x, int y), List<PinDraft>> PinDraftsByXY = new();
				foreach (var p in draft.pins)
				{
					if (PinDraftsByXY.ContainsKey((p.partX, p.partY)))
					{
						PinDraftsByXY[(p.partX, p.partY)].Add(p);
					}
					else
					{
						PinDraftsByXY.Add((p.partX, p.partY), new List<PinDraft>() { p });
					}
				}

				// Create Model Parts
				Part[,] parts = new Part[draft.widthInTiles, draft.heightInTiles];
				foreach (var pinDraft in PinDraftsByXY)
				{
					var realPins = pinDraft.Value.Select(pinDraft => new Pin(pinDraft.name, pinDraft.matterType, pinDraft.side)).ToList();
					parts[pinDraft.Key.x, pinDraft.Key.y] = new Part(realPins);
				}

				// Create Smatrix Connections
				// get all real Pins
				Dictionary<Guid, Pin> ModelPins = new();
				foreach (Part part in parts)
				{
					part.Pins.ForEach(p => ModelPins.Add(p.IDInFlow, p));
					part.Pins.ForEach(p => ModelPins.Add(p.IDOutFlow, p));
				}

				Dictionary<int, PinDraft> PinDraftsByNumber = new();
				draft.pins.ForEach(p => PinDraftsByNumber.Add(p.number, p));
				List<Guid> allPinGuids = new();
				allPinGuids.AddRange(ModelPins.Values.Select(p=>p.IDInFlow).Distinct());
				allPinGuids.AddRange(ModelPins.Values.Select(p=>p.IDOutFlow).Distinct());
				var componentConnectionsRed = new SMatrix(allPinGuids);
				var componentConnectionsGreen = new SMatrix(allPinGuids);
				var componentConnectionsBlue = new SMatrix(allPinGuids);
				var connectionWeightsRed = new Dictionary<(Guid, Guid), Complex>();
				var connectionWeightsGreen = new Dictionary<(Guid, Guid), Complex>();
				var connectionWeightsBlue = new Dictionary<(Guid, Guid), Complex>();
				foreach (Connection connectionDraft in draft.connections)
				{
					var fromPin = PinDraftsByNumber[connectionDraft.fromPinNr];
					var fromModelPin = parts[fromPin.partX, fromPin.partY].Pins.Single(p => p.Side == fromPin.side && p.MatterType == fromPin.matterType);
					var toPin = PinDraftsByNumber[connectionDraft.toPinNr];
					var toModelPin = parts[fromPin.partX, fromPin.partY].Pins.Single(p => p.Side == fromPin.side && p.MatterType == fromPin.matterType);
					var phaseShiftDegreesRed = PhaseShiftCalculator.GetDegrees(connectionDraft.wireLengthNM, PhaseShiftCalculator.laserWaveLengthRedNM);
                    connectionWeightsRed.Add((fromModelPin.IDInFlow, toModelPin.IDOutFlow), Complex.FromPolarCoordinates(connectionDraft.magnitude, phaseShiftDegreesRed));
				};

				componentConnectionsRed.SetValues(connectionWeightsRed);
				// at the moment we pretend that every light is redlaser, but in the future any laserwavelength should be applicable

				modelComponents.Add( new Component(componentConnectionsRed,draft.nazcaFunctionName, draft.nazcaFunctionParameters, parts, typeNumber, DiscreteRotation.R0 ));
				typeNumber++;
			}
			
			ComponentFactory.Instance.InitializeComponentDrafts(modelComponents);
		}
		private void InitializeExternalPortViews(List<ExternalPort> StandardPorts)
		{
			ExternalInputRedTemplate.Visible = false;
			ExternalInputGreenTemplate.Visible = false;
			ExternalInputBlueTemplate.Visible = false;
			ExternalOutputTemplate.Visible = false;

			foreach (var port in StandardPorts)
			{
				TextureRect view;
				if (port is StandardInput input)
				{
					if (input.Color == LightColor.Red)
					{
						view = (TextureRect)ExternalInputRedTemplate.Duplicate();
					}
					else if (input.Color == LightColor.Green)
					{
						view = (TextureRect)ExternalInputGreenTemplate.Duplicate();
					}
					else
					{
						view = (TextureRect)ExternalInputBlueTemplate.Duplicate();
					}
				}
				else
				{
					view = (TextureRect)ExternalOutputTemplate.Duplicate();
				}
				view.Visible = true;
				GridViewModel.GridView.DragDropProxy.AddChild(view);
				view.Position = new Godot.Vector2(view.Position.X - GridView.GlobalPosition.X, (GameManager.TilePixelSize) * port.TilePositionY);
			}
		}
	}
}
