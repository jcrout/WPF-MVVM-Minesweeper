namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Miscellanious;
    using Models;

    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
    {
        protected internal static ISettingsProvider DefaultSettings { get; } = SettingsProvider.Instance;
        protected internal static IMediator DefaultMediator { get; } = WpfMinesweeper.Models.Mediator.Instance;

        private bool disposed;

        public event PropertyChangedEventHandler PropertyChanged;

        internal IMediator Mediator { get; } = ViewModelBase.DefaultMediator;

        internal ISettingsProvider Settings { get; } = ViewModelBase.DefaultSettings;

        protected ViewModelBase()
        {
            
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
