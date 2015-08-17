namespace WpfMinesweeper
{
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    ///     This class encapsulates extension methods for the ICommand
    ///     interface.
    /// </summary>
    [DebuggerStepThrough]
    public static class CommandExtensionMethods
    {
        /// <summary>
        ///     Executes the target ICommand only if the ICommand instance is not null and if the CanExecute(parameter) method
        ///     returns true.
        /// </summary>
        /// <param name="this">The ICommand to execute.</param>
        /// <param name="parameter">
        ///     The data to pass to the ICommand.CanExecute and ICommand.Execute methods.
        ///     This parameter can be null if no data is needed.
        /// </param>
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
