using NLog;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QRSharingApp.Client.Views.FilePreviews
{
    /// <summary>
    /// Interaction logic for VideoPreviewView.xaml
    /// </summary>
    public partial class VideoPreviewView : UserControl
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        public VideoPreviewView()
        {
            InitializeComponent();
            VideoPlayer.PositionChanged += VideoPlayer_PositionChanged;
        }

        private async void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as VideoFilePreviewViewModel;
            if (viewModel != null)
            {
                _logger.Info($"Opening video: {viewModel.FullLocalPath}");
                await VideoPlayer.Open(new Uri(viewModel.FullLocalPath));
                await System.Threading.Tasks.Task.Delay(200);
                LoadingCircle.Visibility = Visibility.Hidden;
                await VideoPlayer.Play();
                _logger.Info($"End opening video: {viewModel.FullLocalPath}");
            }
            else
            {
                _logger.Info($"Start closing video");
                await VideoPlayer.Close();
                _logger.Info($"End closing video");
            }
        }

        private async void VideoPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (VideoPlayer.IsPlaying)
            {
                await VideoPlayer.Pause();
            }
            else if (VideoPlayer.IsPaused)
            {
                await VideoPlayer.Play();
            }
        }

        private void VideoPlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
            if (e.Position == e.EngineState.NaturalDuration)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await VideoPlayer.Seek(TimeSpan.Zero);
                    await VideoPlayer.Play();
                });
            }
        }
    }
}
