﻿using QRSharingApp.ViewModel.Models;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels.Interfaces
{
    public interface ISharedAppDataViewModel : INotifyPropertyChanged
    {
        ActivationStatus ActivationStatus { get; set; }
        bool IsPreviewVisible { get; set; }
        bool IsFilePreviewOpened { get; set; }
        BitmapImage WifiQRImage { get; set; }
        BitmapImage WebUrlQRImage { get; set; }
        Subject<string> NetworkChanged { get; set; }
    }
}