namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Miscellanious;

    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
    {
        private static readonly ISettingsProvider settings = SettingsProvider.Instance;
        private bool disposed;

        public event PropertyChangedEventHandler PropertyChanged;

        public static ISettingsProvider Settings
        {
            get
            {
                return ViewModelBase.settings;
            }
        }

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
                propChangedEvent(this, propChangedEventArgs);
            }
        }
    }
}
