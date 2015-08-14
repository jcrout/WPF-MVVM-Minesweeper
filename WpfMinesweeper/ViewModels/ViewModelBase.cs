namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Miscellanious;
    using Models;

    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
    {
        private bool disposed;
        private IMediator mediator;
        private ISettingsProvider settings;

        protected ViewModelBase()
        {
            this.Mediator = ViewModelBase.DefaultMediator;
            this.Settings = ViewModelBase.DefaultSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal IMediator Mediator
        {
            get
            {
                return this.mediator;
            }
            set
            {
                if (this.mediator != value)
                {
                    if (this.mediator != null)
                    {
                        this.mediator.Unregister(this);
                    }

                    this.mediator = value;
                    this.OnMediatorChanged();
                }
            }
        }

        internal ISettingsProvider Settings
        {
            get
            {
                return this.settings;
            }
            set
            {
                if (this.settings != value)
                {
                    this.settings = value;
                    this.OnSettingsChanged();
                }
            }
        }

        protected internal static IMediator DefaultMediator { get; } = Models.Mediator.Instance;

        protected internal static ISettingsProvider DefaultSettings { get; } = SettingsProvider.Instance;

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

        protected virtual void OnMediatorChanged()
        {
        }

        protected virtual void OnSettingsChanged()
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
