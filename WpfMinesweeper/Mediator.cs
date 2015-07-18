using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMinesweeper.ViewModels;

namespace WpfMinesweeper
{
    public enum ViewModelMessages
    {
        SizeChanged,
        UpdateContainerSize,
        CreateNewBoard,
        GameStarted,
        GameOver,
        UpdateSmileyIndex
    }

    public sealed class Mediator
    {
        private static Mediator instance = new Mediator();
        private Dictionary<ViewModelMessages, List<Action<object>>> callbacks;

        public static Mediator Instance
        {
            get
            {
                return instance;
            }
        }

        private Mediator()
        {
            this.callbacks = new Dictionary<ViewModelMessages, List<Action<object>>>();
        }

        public void Notify(ViewModelMessages message, object parameter = null)
        {
            if (!this.callbacks.ContainsKey(message))
            {
                return;
            }

            foreach (var callback in this.callbacks[message])
            {
                callback(parameter);
            }
        }

        public void Register(ViewModelMessages message, Action<object> callback)
        {
            if (!this.callbacks.ContainsKey(message))
            {
                this.callbacks.Add(message, new List<Action<object>>(1) { callback });
            }
            else
            {
                this.callbacks[message].Add(callback);
            }
        }
    }
}
