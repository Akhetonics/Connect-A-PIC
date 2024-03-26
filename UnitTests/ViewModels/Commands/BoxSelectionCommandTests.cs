using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests;

namespace ConnectAPIC.test.ViewModels.Commands
{
    public class BoxSelectionCommandTests
    {
        private static GridManager InitializeGridWithComponents()
        {
            var grid = new GridManager(10, 10);
            
            grid.PlaceComponent(1, 1, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.PlaceComponent(2, 2, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.PlaceComponent(3, 3, TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON));
            return grid;
        }

        [Theory]
        [InlineData(AppendBehaviors.CreateNew)]
        [InlineData(AppendBehaviors.Append)]
        [InlineData(AppendBehaviors.Remove)]
        public async Task BoxSelectComponentsCommand_AppendBehaviors_Test(AppendBehaviors appendBehavior)
        {
            // Initialisierung
            var gridManager = InitializeGridWithComponents();
            var selectionManager = new SelectionManager(gridManager);
            var command = new BoxSelectComponentsCommand(gridManager, selectionManager);
            selectionManager.Selections.Add( new IntVector(7,8));
            selectionManager.Selections.Add(new IntVector(7, 8));
            var parameters = new BoxSelectComponentsArgs(new IntVector(0, 0), new IntVector(4, 4), appendBehavior);

            await command.ExecuteAsync(parameters);

            // Test the Appendbehaviors
            switch (appendBehavior)
            {
                case AppendBehaviors.CreateNew:
                    Assert.Equal(3, selectionManager.Selections.Count); // expects that only the three new components should be in the list
                    break;
                case AppendBehaviors.Append:
                    Assert.Equal(4, selectionManager.Selections.Count); // expects that all 3 new and the one that was in the list already are in the list
                    break;
                case AppendBehaviors.Remove:
                    Assert.Equal(1, selectionManager.Selections.Count); // expects, that only the one that was initialy in the list remains
                    break;
            }
        }

    }
}
