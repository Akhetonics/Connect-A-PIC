using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ToolBox;

namespace ConnectAPIC.Scripts.ViewModel
{
    public class ToolViewModel : INotifyPropertyChanged
    {
        
        private ObservableCollection<ITool> tools = new ();
        public ObservableCollection<ITool> Tools {
            get => tools;
            set {
                tools = value;
                OnPropertyChanged();
            }
        }
        private ITool currentTool;
        public ITool CurrentTool
        {
            get { return currentTool; }
            set {
                currentTool = value;
                OnPropertyChanged();
            }
        }

        public GridManager Grid { get; }

        public event PropertyChangedEventHandler PropertyChanged;


        public ToolViewModel(GridManager grid)
        {
            Grid = grid;
        }

        public void SetCurrentTool(ITool tool)
        {
            if (CurrentTool != null)
            {
                CurrentTool.Exit();
            }
            CurrentTool = tool;
            CurrentTool.Initialize(this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
