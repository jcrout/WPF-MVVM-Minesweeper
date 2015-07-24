namespace WpfMinesweeper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Implementation of ICommand that takes in an execution delegate with or without an object parameter. Optionally, you can specify a CanExecute delegate that either includes or omits an object paramter.
    /// </summary>
    [DebuggerStepThrough]
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
        /// <summary>
        /// Executes the command only if the command isn't null and if the CanExecute(paramter) method returns true;
        /// </summary>
        /// <param name="this">The command to execute. This paramter can be null.</param>
        /// <param name="parameter">The paramter to pass to the command. This paramter can be omitted or set to null.</param>
        [DebuggerStepThrough]
        public static void ExecuteIfAbleTo(this ICommand @this, object parameter = null)
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
