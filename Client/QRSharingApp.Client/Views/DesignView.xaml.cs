using HandyControl.Controls;
using QRSharingApp.ViewModel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QRSharingApp.Client.Views
{
    /// <summary>
    /// Interaction logic for DesignView.xaml
    /// </summary>
    public partial class DesignView : UserControl
    {
        private int _counter = 0;

        public DesignView()
        {
            InitializeComponent();
        }

        private void CheckComboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var checkComboBox = sender as CheckComboBox;
            var dataContext = e.NewValue as DesignViewModel;
            if (dataContext == null)
            {
                return;
            }

            if (dataContext.AllFieldsWereRequired)
            {
                checkComboBox.SelectionChanged += CheckComboBox_SelectionChanged;
            }
        }

        private void CheckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                _counter++;
            }

            if (_counter == 3)
            {
                var dataContext = DataContext as DesignViewModel;
                foreach (var item in dataContext.FormRequiredFields)
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}
