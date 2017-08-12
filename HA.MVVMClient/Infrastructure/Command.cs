using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HA.MVVMClient.Infrastructure
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Func<object, bool> canExecute; 
        private readonly Action<object> executeAction;

        public Command(Action executeAction, Func<bool> canExecute = null)
        {
            this.executeAction = (c) => executeAction();
            if (canExecute!=null)
                this.canExecute = (c) => canExecute();
        }

        public Command(Action<object> executeAction, Func<object, bool> canExecute = null) 
        { 
            this.executeAction = executeAction; 
            if (canExecute!=null)
                this.canExecute = canExecute; 
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {      
            if (canExecute != null)
                return canExecute(parameter);            
            return true;
        }

        public void Execute(object parameter)
        {
            executeAction(parameter); 
        }
    }
}
