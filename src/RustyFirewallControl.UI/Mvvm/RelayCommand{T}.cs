using System;
using System.Windows.Input;

namespace RustyFirewallControl.UI.Mvvm
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
            => canExecute?.Invoke((T)parameter) != false;

        public void Execute(object parameter)
            => execute((T)parameter);
    }
}