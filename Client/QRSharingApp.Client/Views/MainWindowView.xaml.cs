using System.Windows;
using System.Windows.Controls;
using Unity;

namespace QRSharingApp.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(IUnityContainer unityContainer)
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ((RadioButton)sender).IsChecked = false;
        }
    }
}
