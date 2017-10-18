using System;
using System.Windows.Input;

namespace JaCoCoReader.UI
{
    public class TargetCommand<T> : ICommand
    {
        private readonly CommandCanExecuteDelegate<T> _canExecute;
        private readonly CommandDelegate<T> _method;

        public TargetCommand(CommandDelegate<T> method)
        {
            _method = method;
        }

        public TargetCommand(CommandDelegate<T> method, CommandCanExecuteDelegate<T> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            if (parameter is T)
            {
                _method.Invoke((T)parameter);
            }
            else
            {
                _method.Invoke(default(T));
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
                return _canExecute((T)parameter);
            }
            return _canExecute(default(T));
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }

    public class TargetCommand : ICommand
    {
        private readonly CommandCanExecuteDelegate _canExecute;
        private readonly CommandDelegate _method;

        public TargetCommand(CommandDelegate method)
        {
            _method = method;
        }

        public TargetCommand(CommandDelegate method, CommandCanExecuteDelegate canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            _method.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}