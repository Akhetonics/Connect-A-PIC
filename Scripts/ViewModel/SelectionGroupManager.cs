using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel;
using System.Collections.ObjectModel;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class SelectionTool
    {
        public class SelectionGroupManager
        {
            public SelectionGroupManager(GridViewModel viewModel, SelectionManager selectionManager)
            {
                ViewModel = viewModel;
                SelectionManager = selectionManager;
                BoxSelectComponentsCommand = new BoxSelectComponentsCommand(viewModel.Grid, selectionManager);
            }

            public GridViewModel ViewModel { get; }
            public SelectionManager SelectionManager { get; }

            public UniqueObservableCollection<IntVector> SelectedComponents => SelectionManager.Selections;
            public BoxSelectComponentsCommand BoxSelectComponentsCommand { get; private set; }
        }
    }
}
