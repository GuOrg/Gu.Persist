namespace Gu.Persist.Demo
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Func<object?, bool> canExecute;

        public RelayCommand(Action<object?> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? (_ => true);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute(parameter);
        }

        public virtual void Execute(object? parameter)
        {
            this.execute(parameter);
        }
    }
}