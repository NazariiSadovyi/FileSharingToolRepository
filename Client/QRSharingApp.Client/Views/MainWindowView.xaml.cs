using System.Windows;
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
    }
}
