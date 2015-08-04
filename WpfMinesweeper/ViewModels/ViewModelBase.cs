namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JonUtility;
    using System.Windows.Input;
    using Miscellanious;

    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
    {
        private static readonly ISettingsProvider settings = SettingsProvider.Instance;

        public static ISettingsProvider Settings
        {
            get
            {
                return ViewModelBase.settings;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool disposed;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.OnDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing)
        {
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (string.IsNullOrWhiteSpace(prop))
            {
                return;
            }

            var propChangedEvent = this.PropertyChanged;
            if (propChangedEvent != null)
            {
                var propChangedEventArgs = new PropertyChangedEventArgs(prop);
                propChangedEvent(this,
                    propChangedEventArgs);
            }
        }
    }
}