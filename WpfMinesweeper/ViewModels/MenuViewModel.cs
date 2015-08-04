﻿namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Xceed.Wpf.Toolkit;

    public class MenuViewModel : ViewModelBase
    {
        private static readonly Color defaultSelectedColor = ViewModelBase.Settings.TileColor;

        private ViewModelBase customBoardViewModel;
        private ViewModelBase statisticsViewModel;
        private Color selectedColor;
        private ICommand boardSizeCommand;
        private ICommand tileColorCommand;

        public MenuViewModel()
        {
            this.boardSizeCommand = new Command(o => this.OnBoardSizeSelected(o));
            this.selectedColor = MenuViewModel.defaultSelectedColor;
            this.CustomBoardViewModel = new CustomBoardViewModel();
            this.StatisticsViewModel = new StatisticsViewModel();
        }

        public ICommand BoardSizeCommand
        {
            get
            {
                return this.boardSizeCommand;
            }
            set
            {
                if (this.boardSizeCommand != value)
                {
                    this.boardSizeCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand TileColorCommand
        {
            get
            {
                return this.tileColorCommand;
            }
            set
            {
                if (this.tileColorCommand != value)
                {
                    this.tileColorCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Color SelectedTileColor
        {
            get
            {
                return this.selectedColor;
            }
            set
            {
                if (this.selectedColor != value)
                {
                    this.selectedColor = value;
                    ViewModelBase.Settings.TileColor = value;
                    var brush = new SolidColorBrush(value);
                    Mediator.Instance.Notify(ViewModelMessages.TileColorsChanged,
                        brush);
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase CustomBoardViewModel
        {
            get
            {
                return this.customBoardViewModel;
            }
            set
            {
                if (this.customBoardViewModel != value)
                {
                    this.customBoardViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase StatisticsViewModel
        {
            get
            {
                return this.statisticsViewModel;
            }
            set
            {
                if (this.statisticsViewModel != value)
                {
                    this.statisticsViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnBoardSizeSelected(object paramter)
        {
            Mediator.Instance.Notify(ViewModelMessages.CreateNewBoard,
                paramter);
        }
    }
}