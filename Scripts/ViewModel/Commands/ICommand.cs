using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public interface ICommand
    {
        public bool CanExecute(object parameter);
        public Task ExecuteAsync(object parameter);
        public void Undo();
        public void Redo();
        bool CanMergeWith(ICommand other);
        void MergeWith(ICommand other);
        event EventHandler Executed;
    }
}
