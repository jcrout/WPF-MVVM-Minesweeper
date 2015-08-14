
namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using JonUtility;

    public class GradientViewModel : MinesweeperComponentViewModel
    {
        private ObservableCollection<GradientStopProxy> gradientStops = new ObservableCollection<GradientStopProxy>();
        private GradientBrush selectedBrush;
        private ICommand addGradientStopCommand;
        private ICommand removeGradientStopCommand;
        private bool isLinear = true;
        private bool isRadial = false;
        private bool bothTypeOptionsUpdated = false;

        public GradientViewModel()
        {
            this.AddStop(Colors.Red, 0d);
            this.AddStop(Colors.Black, 1d);
            this.AddGradientStopCommand = new Command(this.OnAddGradientStop);
            this.RemoveGradientStopCommand = new Command(this.OnRemoveGradientStop, () => this.gradientStops.Count > 1);
            this.SelectedBrush = GetNewBrush();
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

        private void OnAddGradientStop()
        {
            this.AddStop(Colors.White, 1d);
            this.UpdateBrushes();
        }

        private void OnRemoveGradientStop()
        {
            var gradientStop = this.gradientStops[this.gradientStops.Count - 1];
            gradientStop.PropertyChanged -= GradientStop_PropertyChanged;
            this.gradientStops.Remove(gradientStop);
            this.UpdateBrushes();
        }

        private void AddStop(Color color, double offset)
        {
            var gradientStop = new GradientStopProxy(color, offset);
            gradientStop.PropertyChanged += GradientStop_PropertyChanged;
            this.gradientStops.Add(gradientStop);
        }

        private void GradientStop_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateBrushes();
        }

        private void UpdateBrushes()
        {
            this.SelectedBrush = GetNewBrush();
            this.Mediator.Notify(Models.ViewModelMessages.TileColorsChanged, this.GetNewBrush());
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

        public class GradientStopProxy : INotifyPropertyChanged
        {
            private double offset;
            private Color color;

            public event PropertyChangedEventHandler PropertyChanged;

            public double Offset
            {
                get { return offset; }
                set
                {
                    offset = value;
                    this.PropertyChanged.SafeRaise(this, new PropertyChangedEventArgs(nameof(this.Offset)));
                }
            }

            public Color Color
            {
                get { return color; }
                set
                {
                    color = value;
                    this.PropertyChanged.SafeRaise(this, new PropertyChangedEventArgs(nameof(this.Color)));
                }
            }

            public GradientStopProxy(Color color, double offset)
            {
                this.color = color;
                this.offset = offset;
            }
        }
    }
}
