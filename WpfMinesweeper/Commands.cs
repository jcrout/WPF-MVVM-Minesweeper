using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfMinesweeper
{
    public class Command : ICommand
    {
        private Action execute;
        private Action<object> executeWithArgument;
        private Func<bool> canExecute;
        private Func<object, bool> canExecuteWithArgument;

        public Command(Action<object> executionDelegate, Func<object, bool> canExecuteDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.executeWithArgument = executionDelegate;
            this.canExecuteWithArgument = canExecuteDelegate;
        }

        public Command(Action<object> executionDelegate, Func<bool> canExecuteDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.executeWithArgument = executionDelegate;
            this.canExecute = canExecuteDelegate;
        }

        public Command(Action executionDelegate, Func<object, bool> canExecuteDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.execute = executionDelegate;
            this.canExecuteWithArgument = canExecuteDelegate;
        }

        public Command(Action executionDelegate, Func<bool> canExecuteDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.execute = executionDelegate;
            this.canExecute = canExecuteDelegate;
        }

        public Command(Action<object> executionDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.executeWithArgument = executionDelegate;
        }

        public Command(Action executionDelegate)
        {
            if (executionDelegate == null)
            {
                throw new ArgumentNullException("executionDelegate");
            }

            this.execute = executionDelegate;
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                if (this.canExecuteWithArgument == null)
                {
                    return true;
                }
                else
                {
                    return this.canExecuteWithArgument(parameter);
                }                
            }
            else
            {
                return this.canExecute();                              
            }
        }

        public void Execute(object parameter)
        {
           if (this.execute == null)
           {
               this.executeWithArgument(parameter);
           }
           else
           {
               this.execute();
           }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public static class ICommandExtensionMethods
    {
        public static void ExecuteIfAbleTo(this ICommand @this, object parameter)
        {
            if (@this == null)
            {
                return;
            }

            if (@this.CanExecute(parameter))
            {
                @this.Execute(parameter);
            }
        }
    }
}
