using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
