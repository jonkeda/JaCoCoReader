using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JaCoCoReader.Core.UI
{
    public class TargetTaskCommand<T> : ICommand
    {
        private readonly CommandTaskCanExecuteDelegate<T> _canExecute;
        private readonly CommandTaskDelegate<T> _method;

        public TargetTaskCommand(CommandTaskDelegate<T> method)
        {
            _method = method;
        }

        public TargetTaskCommand(CommandTaskDelegate<T> method, CommandTaskCanExecuteDelegate<T> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            if (parameter is T)
            {
                Task task = _method((T)parameter);
                if (task.Status == TaskStatus.Created)
                {
                    task.Start();
                }
            }
            else
            {
                Task task = _method(default(T));
                if (task.Status == TaskStatus.Created)
                {
                    task.Start();
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            if (parameter is T)
            {
                Task<bool> task = _canExecute((T)parameter);
                task.Wait();
                return task.Result;
            }
            else
            {
                Task<bool> task = _canExecute(default(T));
                task.Wait();
                return task.Result;
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }

    public class TargetTaskCommand : ICommand
    {
        private readonly CommandTaskCanExecuteDelegate _canExecute;
        private readonly CommandTaskDelegate _method;

        public TargetTaskCommand(CommandTaskDelegate method)
        {
            _method = method;
        }

        public TargetTaskCommand(CommandTaskDelegate method, CommandTaskCanExecuteDelegate canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            Task task = _method();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                Task<bool> task = _canExecute();
                task.Wait();
                return task.Result;
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}