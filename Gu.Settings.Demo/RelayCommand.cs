namespace Gu.Settings.Demo.RemoveItemBox
{
    using System;
    using System.Windows.Input;
    using Core;

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Ensure.NotNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute ?? (_ => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public virtual void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}