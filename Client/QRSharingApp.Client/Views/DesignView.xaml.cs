using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using QRSharingApp.ViewModel.ViewModels;
using System;
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

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = e.NewValue as DesignViewModel;
            if (dataContext == null)
            {
                return;
            }

            switch (dataContext.AppSettingService.CultureName)
            {
                case "en-US":
                    USLanguageRadioButton.IsChecked = true;
                    break;
                case "ru-RU":
                    RULanguageRadioButton.IsChecked = true;
                    break;
                //case "es-ES":
                //    ESLanguageRadioButton.IsChecked = true;
                //    break;
                default:
                    break;
            }

            switch (dataContext.AppSettingService.SkinType)
            {
                case "white":
                    WhiteApplicationStyleRadioButton.IsChecked = true;
                    break;
                case "dark":
                    BlackApplicationStyleRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            switch (radioButton.Name)
            {
                case "WhiteApplicationStyleRadioButton":
                    ((App)Application.Current).UpdateSkin(SkinType.Default);
                    ((DesignViewModel)DataContext).AppSettingService.SkinType = "white";
                    break;
                case "BlackApplicationStyleRadioButton":
                    ((App)Application.Current).UpdateSkin(SkinType.Dark);
                    ((DesignViewModel)DataContext).AppSettingService.SkinType = "dark";
                    break;
                default:
                    break;
            }
        }
    }
}
