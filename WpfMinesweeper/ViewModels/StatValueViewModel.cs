namespace WpfMinesweeper.ViewModels
{
    using System.Collections.Generic;

    public class StatValueViewModel : ViewModelBase
    {
        private string name;
        private object selectedValue;
        private IEnumerable<object> statisticValues;

        public StatValueViewModel(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public object SelectedValue
        {
            get
            {
                return this.selectedValue;
            }
            set
            {
                if (this.selectedValue != value)
                {
                    this.selectedValue = value;

                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<object> StatisticValues
        {
            get
            {
                return this.statisticValues;
            }
            set
            {
                if (this.statisticValues != value)
                {
                    this.statisticValues = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
