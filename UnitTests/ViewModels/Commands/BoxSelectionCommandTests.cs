using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnitTests.Helpers.GridHelpers;
using UnitTests;
using UnitTests.Grid;

namespace ConnectAPIC.test.ViewModels.Commands
{
    public class BoxSelectionCommandTests
    {
        [Theory]
        [InlineData(AppendBehaviors.CreateNew)]
        [InlineData(AppendBehaviors.Append)]
        [InlineData(AppendBehaviors.Remove)]
        public async Task BoxSelectComponentsCommand_AppendBehaviors_Test(AppendBehaviors appendBehavior)
        {
            // Initialization
            var gridManager = GridHelpers.InitializeGridWithComponents();
            var selectionManager = new SelectionManager(gridManager);
            var command = new BoxSelectComponentsCommand(gridManager, selectionManager);
            selectionManager.Selections.Add( new IntVector(7,8));
            selectionManager.Selections.Add(new IntVector(7, 8));
            var parameters = new BoxSelectComponentsArgs(new() { (GridStart: new IntVector(0, 0), GridEnd: new IntVector(4, 4)) }, appendBehavior);

            await command.ExecuteAsync(parameters);

            // Test the AppendBehaviors
            switch (appendBehavior)
            {
                case AppendBehaviors.CreateNew:
                    Assert.Equal(3, selectionManager.Selections.Count); // expects that only the three new components should be in the list
                    break;
                case AppendBehaviors.Append:
                    Assert.Equal(4, selectionManager.Selections.Count); // expects that all 3 new and the one that was in the list already are in the list
                    break;
                case AppendBehaviors.Remove:
                    Assert.Equal(1, selectionManager.Selections.Count); // expects, that only the one that was initially in the list, remains
                    break;
            }
        }

        [Fact]
        public async Task TestDownToUpSelectionBox()
        {
            // arrange
            var gridManager = GridHelpers.InitializeGridWithComponents();
            var selectionManager = new SelectionManager(gridManager);
            var command = new BoxSelectComponentsCommand(gridManager, selectionManager);
            var parameters = new BoxSelectComponentsArgs(new() { (GridStart: new IntVector(4, 4), GridEnd: new IntVector(0, 0)) }, AppendBehaviors.CreateNew);
            var components = FindAllComponentsInGrid(gridManager);

            // act
            await command.ExecuteAsync(parameters);
            
            // assert
            selectionManager.Selections.Count.ShouldBe(components.Count);
        }

        [Fact]
        public async void TestRevertBoxSelection()
        {
            // initialize
            var gridManager = GridHelpers.InitializeGridWithComponents();
            var components = FindAllComponentsInGrid(gridManager);
            var initiallySelectedComponent = components.ToArray()[1];
            var selectionManager = new SelectionManager(gridManager);
            selectionManager.Selections.Add(new IntVector(initiallySelectedComponent.GridXMainTile, initiallySelectedComponent.GridYMainTile));
            var command = new BoxSelectComponentsCommand(gridManager, selectionManager);
            var parameters = new BoxSelectComponentsArgs(new() { (GridStart: new IntVector(4, 4), GridEnd: new IntVector(0, 0)) }, AppendBehaviors.CreateNew);
            selectionManager.Selections.Add(new IntVector(components.First().GridXMainTile, components.First().GridYMainTile));

            // act
            await command.ExecuteAsync(parameters);
            var selectedElementCount = selectionManager.Selections.Count();

            command.Undo();

            // assert
            selectedElementCount.ShouldBe(components.Count);
            selectionManager.Selections.First().ShouldBe(new IntVector(initiallySelectedComponent.GridXMainTile, initiallySelectedComponent.GridYMainTile));
        }
    }
}
