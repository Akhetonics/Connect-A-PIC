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
        
    }
}
