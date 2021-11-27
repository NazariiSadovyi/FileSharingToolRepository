using FST.Client.Helpers;
using FST.ViewModel.ViewModels;
using System.ComponentModel;
using System.Linq;
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
        private MediaElement VideoPlayer => this.FindVisualChildren<MediaElement>().FirstOrDefault();

        public GridFilePreviewView()
        {
            InitializeComponent();

            var viewModel = DataContext as GridFilePreviewViewModel;
            viewModel.SharedAppDataViewModel.PropertyChanged += SharedAppDataViewModel_PropertyChanged;
        }

        private void SharedAppDataViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SharedAppDataViewModel.IsPreviewVisible):
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        var viewModel = DataContext as GridFilePreviewViewModel;
                        if (!viewModel.SharedAppDataViewModel.IsPreviewVisible)
                        {
                            VideoPlayer?.Stop();
                        }
                        else
                        {
                            VideoPlayer?.Play();
                        }
                    });
                    break;
                default:
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            var hwndTarget = hwndSource.CompositionTarget;
            hwndTarget.RenderMode = RenderMode.SoftwareOnly;
        }

        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            VideoPlayer?.Play();
        }

        private void MediaElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoPlayer?.Pause();
        }
    }
}