using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public interface ICommand : ICommandBase, IUndoableCommand, IMergeAbleCommand
    {
    }
    public interface ICommandBase
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
        void MergeWith(ICommand other);
    }

    public abstract class CommandBase<T> : ICommand
    {
        public T ExecutionParams { get; internal set; }

        protected List<Exception> Errors = new();

        public event EventHandler Executed;
        internal abstract Task ExecuteAsyncCmd(T parameter);
        public virtual bool CanExecute(object parameter)
        {
            return parameter is T;
        }

        public virtual bool CanMergeWith(ICommand other)
        {
            return false;
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return;
            ExecutionParams = (T)parameter;
            await ExecuteAsyncCmd((T)parameter);
            Executed?.Invoke(this, new ExecutionResult(Errors));
            return;
        }

        public virtual void MergeWith(ICommand other)
        {
        }

        public virtual void Redo()
        {
            if (ExecutionParams == null) return;
            ExecuteAsync(ExecutionParams).Wait();
        }

        public virtual void Undo()
        {
        }

        public virtual void ClearErrors()
        {
            Errors.Clear();
        }
    }

    public class ExecutionResult : EventArgs
    {
        public List<Exception> Errors { get; private set; }

        public ExecutionResult(List<Exception> errors)
        {
            Errors = errors;
        }
    }
}
