namespace WpfMinesweeper.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Media;
    using JonUtility;
    using Models;

    public class GradientViewModel : MinesweeperComponentViewModel
    {
        private ICommand addGradientStopCommand;
        private bool bothTypeOptionsUpdated;
        private ObservableCollection<GradientStopProxy> gradientStops = new ObservableCollection<GradientStopProxy>();
        private bool isLinear = true;
        private bool isRadial;
        private ICommand removeGradientStopCommand;
        private GradientBrush selectedBrush;

        public GradientViewModel()
        {
            this.AddStop(Colors.Red, 0d);
            this.AddStop(Colors.Black, 1d);
            this.AddGradientStopCommand = new Command(this.OnAddGradientStop);
            this.RemoveGradientStopCommand = new Command(this.OnRemoveGradientStop, () => this.gradientStops.Count > 1);
            this.SelectedBrush = this.GetNewBrush();
        }

        public ICommand AddGradientStopCommand
        {
            get
            {
                return this.addGradientStopCommand;
            }
            set
            {
                if (this.addGradientStopCommand != value)
                {
                    this.addGradientStopCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<GradientStopProxy> GradientStops
        {
            get
            {
                return this.gradientStops;
            }
            set
            {
                if (this.gradientStops != value)
                {
                    this.gradientStops = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsLinear
        {
            get
            {
                return this.isLinear;
            }
            set
            {
                if (this.isLinear != value)
                {
                    this.isLinear = value;
                    this.BrushTypeChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsRadial
        {
            get
            {
                return this.isRadial;
            }
            set
            {
                if (this.isRadial != value)
                {
                    this.isRadial = value;
                    this.BrushTypeChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand RemoveGradientStopCommand
        {
            get
            {
                return this.removeGradientStopCommand;
            }
            set
            {
                if (this.removeGradientStopCommand != value)
                {
                    this.removeGradientStopCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public GradientBrush SelectedBrush
        {
            get
            {
                return this.selectedBrush;
            }
            set
            {
                if (this.selectedBrush != value)
                {
                    this.selectedBrush = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void AddStop(Color color, double offset)
        {
            var gradientStop = new GradientStopProxy(color, offset);
            gradientStop.PropertyChanged += this.GradientStop_PropertyChanged;
            this.gradientStops.Add(gradientStop);
        }

        private void BrushTypeChanged()
        {
            if (!this.bothTypeOptionsUpdated)
            {
                this.bothTypeOptionsUpdated = true;
                return;
            }

            this.UpdateBrushes();
            this.bothTypeOptionsUpdated = false;
        }

        private GradientBrush GetNewBrush()
        {
            var stops = this.gradientStops.Select(gs => new GradientStop(gs.Color.WithAlpha(150), gs.Offset));
            var collection = new GradientStopCollection(stops);
            GradientBrush brush;

            if (this.IsLinear)
            {
                brush = new LinearGradientBrush(collection);
            }
            else if (this.isRadial)
            {
                brush = new RadialGradientBrush(collection);
            }
            else
            {
                return null;
            }

            brush.Freeze();
            return brush;
        }

        private void GradientStop_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateBrushes();
        }

        private void OnAddGradientStop()
        {
            this.AddStop(Colors.White, 1d);
            this.UpdateBrushes();
        }

        private void OnRemoveGradientStop()
        {
            var gradientStop = this.gradientStops[this.gradientStops.Count - 1];
            gradientStop.PropertyChanged -= this.GradientStop_PropertyChanged;
            this.gradientStops.Remove(gradientStop);
            this.UpdateBrushes();
        }

        private void UpdateBrushes()
        {
            this.SelectedBrush = this.GetNewBrush();
            this.Mediator.Notify(ViewModelMessages.TileColorsChanged, this.GetNewBrush());
        }

        public class GradientStopProxy : INotifyPropertyChanged
        {
            private Color color;
            private double offset;

            public GradientStopProxy(Color color, double offset)
            {
                this.color = color;
                this.offset = offset;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public Color Color
            {
                get
                {
                    return this.color;
                }
                set
                {
                    this.color = value;
                    this.PropertyChanged.SafeRaise(this, new PropertyChangedEventArgs(nameof(this.Color)));
                }
            }

            public double Offset
            {
                get
                {
                    return this.offset;
                }
                set
                {
                    this.offset = value;
                    this.PropertyChanged.SafeRaise(this, new PropertyChangedEventArgs(nameof(this.Offset)));
                }
            }
        }
    }
}
