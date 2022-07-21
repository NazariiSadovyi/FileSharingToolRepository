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
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as VideoFilePreviewViewModel;
            if (viewModel != null)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        _logger.Info($"Opening video: {viewModel.ThumbnailViewModel.FullLocalPath}");
                        await VideoPlayer.Open(new Uri(viewModel.ThumbnailViewModel.FullLocalPath));
                        _logger.Info($"End opening video: {viewModel.ThumbnailViewModel.FullLocalPath}");
                        await System.Threading.Tasks.Task.Delay(200);
                        _logger.Info($"Start playing video: {viewModel.ThumbnailViewModel.FullLocalPath}");
                        await VideoPlayer.Play();
                        _logger.Info($"End playing video: {viewModel.ThumbnailViewModel.FullLocalPath}");
                        LoadingCircle.Visibility = Visibility.Hidden;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Opening video exception");
                    }
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        _logger.Info($"Start closing video");
                        await VideoPlayer.Close();
                        _logger.Info($"End closing video");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Closing video exception");
                    }
                });
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

        private void VideoPlayer_MediaEnded(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await VideoPlayer.Seek(new TimeSpan(0, 0, 0, 0, 10));
                    await VideoPlayer.Play();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Loop video exception");
                }
            });
        }
    }
}
