﻿using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public abstract class FilePreviewBaseViewModel : ViewModelBase
    {
        [Dependency]
        public IAppSettingService AppSettingService;

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsLoading { get; set; }
        public string LocalPath { get; set; }
        public string FullLocalPath { get; set; }
        public BitmapImage QRImage { get; set; }
        public string SharedLink { get; set; }

        public string BackgroundImagePath { get; set; }

        public abstract Task OnLoadDataAsync();

        public FilePreviewBaseViewModel(ThumbnailViewModel thumbnailViewModel)
        {
            Id = thumbnailViewModel.Name;
            Name = thumbnailViewModel.Name;
            LocalPath = thumbnailViewModel.LocalPath;
            FullLocalPath = thumbnailViewModel.FullLocalPath;
            QRImage = thumbnailViewModel.QRImage;
            SharedLink = thumbnailViewModel.SharedLink;
        }

        public async Task OnLoadAsync()
        {
            IsLoading = true;
            try
            {
                BackgroundImagePath = AppSettingService.BackgroundImagePath;
                await OnLoadDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
