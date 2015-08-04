namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// This class extends the ListBox class to allow XAML binding to the SelectedItems property through the new SelectedItemsBinding property.
    /// </summary>
    public class ListBox2 : ListBox
    {
        /// <summary>
        /// This property is a proxy to SelectedItems and updates any bindings.
        /// </summary>
        public static DependencyProperty SelectedItemsBindingProperty = DependencyProperty.Register(
            "SelectedItemsBinding",
            typeof (IList),
            typeof (ListBox2),
            new PropertyMetadata(
                new List<object>(),
                ListBox2.SelectedItemsBindingChanged));

        /// <summary>
        /// Gets or sets the SelectedItems property. This property is a proxy to SelectedItems and updates any bindings.
        /// </summary>
        public IList SelectedItemsBinding
        {
            get
            {
                return (IList) this.GetValue(ListBox2.SelectedItemsBindingProperty);
            }
            set
            {
                this.SetValue(ListBox2.SelectedItemsBindingProperty,
                    value);
            }
        }

        /// <summary>
        /// This method updates the SelectedItemsBinding property after any changes to the ListBox's SelectedItems property.
        /// </summary>
        /// <param name="e">The SelectionChanged EventArgs.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.SelectedItemsBinding = this.SelectedItems;
        }

        /// <summary>
        /// PropertyChangedCallback implementation for SelectedItemsBindingProperty.
        /// </summary>
        /// <param name="d">The System.Windows.DependencyObject on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void SelectedItemsBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox2 = (ListBox2) d;
            var newList = (IEnumerable) e.NewValue;

            if (!object.ReferenceEquals(listBox2.SelectedItems,
                newList))
            {
                listBox2.SetSelectedItems(newList);
            }
        }
    }
}