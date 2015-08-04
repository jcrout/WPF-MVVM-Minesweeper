﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMinesweeper.Properties;
using WpfMinesweeper.ViewModels;

namespace WpfMinesweeper
{
    public enum ViewModelMessages
    {
        TileBoardSizeChanged,
        CreateNewBoard,
        GameStarted,
        GameOver,
        Victory,
        UpdateSmileyIndex,
        LeftMouseUp,
        RightMouseUp,
        TileColorsChanged,
        TileSizeChanged,
        GameWindowStateChanged,
        StatisticsLoaded
    }

    public sealed class Mediator
    {
        private static readonly Mediator instance = new Mediator();
        private readonly Dictionary<ViewModelMessages, List<Action<object>>> callbacks;

        public static Mediator Instance
        {
            get
            {
                return Mediator.instance;
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
                this.callbacks.Add(message,
                    new List<Action<object>>(1) {callback});
            }
            else
            {
                this.callbacks[message].Add(callback);
            }
        }
    }
}