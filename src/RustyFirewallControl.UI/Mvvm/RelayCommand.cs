using System;
using System.Windows.Input;

namespace RustyFirewallControl.UI.Mvvm
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
            => canExecute?.Invoke() != false;

        public void Execute(object parameter)
            => execute();
    }
}
