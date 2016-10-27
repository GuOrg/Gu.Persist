namespace Gu.Persist.Demo
{
    using System;
    using System.Windows.Input;

    using Gu.Persist.Core;

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Ensure.NotNull(execute, nameof(execute));

            this.execute = execute;
            this.canExecute = canExecute ?? (_ => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute(parameter);
        }

        public virtual void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}