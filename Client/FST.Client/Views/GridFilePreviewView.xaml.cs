using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace FST.Client.Views
{
    /// <summary>
    /// Interaction logic for GridFilePreview.xaml
    /// </summary>
    public partial class GridFilePreviewView : UserControl
    {
        public GridFilePreviewView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            var hwndTarget = hwndSource.CompositionTarget;
            hwndTarget.RenderMode = RenderMode.SoftwareOnly;
        }
    }
}