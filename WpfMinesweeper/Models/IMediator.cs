namespace WpfMinesweeper.Models
{
    using System;

    public interface IMediator
    {
        void Notify(ViewModelMessages message, object parameter = null);

        void Register(ViewModelMessages message, Action<object> callback);

        void Unregister(object objectToUnregister);
    }
}
