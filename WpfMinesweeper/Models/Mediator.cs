namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class Mediator : IMediator
    {
        private static readonly Mediator instance = new Mediator();
        private readonly Dictionary<ViewModelMessages, List<Action<object>>> callbacks;

        private Mediator()
        {
            this.callbacks = new Dictionary<ViewModelMessages, List<Action<object>>>();
        }

        /// <summary>
        ///     Gets the current Mediator instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Mediator Instance
        {
            get
            {
                return Mediator.instance;
            }
        }

        public void Notify(ViewModelMessages message, object parameter = null)
        {
            if (!this.callbacks.ContainsKey(message))
            {
                return;
            }

            foreach (var callback in this.callbacks[message].ToArray())
            {
                callback(parameter);
            }
        }

        public void Register(ViewModelMessages message, Action<object> callback)
        {
            if (!this.callbacks.ContainsKey(message))
            {
                this.callbacks.Add(message, new List<Action<object>>(1) {callback});
            }
            else
            {
                this.callbacks[message].Add(callback);
            }
        }

        public void Unregister(object objectToUnregister)
        {
            foreach (var kvp in this.callbacks)
            {
                foreach (var callback in kvp.Value.ToArray())
                {
                    if (object.ReferenceEquals(callback.Target, objectToUnregister))
                    {
                        kvp.Value.Remove(callback);
                    }
                }
            }
        }
    }
}
