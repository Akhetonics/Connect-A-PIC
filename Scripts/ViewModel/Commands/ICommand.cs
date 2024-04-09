using ConnectAPIC.LayoutWindow.ViewModel.Commands;
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
        event EventHandler Executed;
    }
    public interface IUndoableCommand
    {
        public void Undo();
        public void Redo();
    }
    public interface IMergeAbleCommand
    {
        bool CanMergeWith(ICommand other);
        void MergeWith (ICommand other);
    }

    public abstract class CommandBase<T> : ICommand, IUndoableCommand , IMergeAbleCommand
    {
        public T ExecutionParams { get; internal set; }

        public event EventHandler Executed;
        internal abstract Task ExecuteAsyncCmd(T parameter);
        public virtual bool CanExecute (object parameter)
        {
            return true;
        }

        public virtual bool CanMergeWith(ICommand other)
        {
            return false;
        }

        public Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return Task.CompletedTask;
            ExecutionParams = (T)parameter;
            ExecuteAsyncCmd((T)parameter);
            Executed?.Invoke(this, new EventArgs());
            return Task.CompletedTask;
        }

        public virtual void MergeWith(ICommand other)
        {
        }

        public virtual void Redo()
        {
            if (ExecutionParams != null) return;
            ExecuteAsync(ExecutionParams).Wait();
        }

        public virtual void Undo()
        {
        }
    }
}
