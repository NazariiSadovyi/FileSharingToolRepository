using System.Windows;

namespace FST.ViewModel.Interfaces
{
    public interface IMainWindowViewModel
    {
        WindowStyle WindowStyle { get; set; }
        WindowState WindowState { get; set; }
        ResizeMode ResizeMode { get; set; }
    }
}
