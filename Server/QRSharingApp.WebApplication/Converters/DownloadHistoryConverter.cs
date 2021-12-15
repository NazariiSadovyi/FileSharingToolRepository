using QRSharingApp.DataAccess.Entities;
using QRSharingApp.WebApplication.Models;
using System;

namespace QRSharingApp.WebApplication.Converters
{
    public static class DownloadHistoryConverter
    {
        public static DownloadHistoryViewModel ToViewModel(this DownloadHistory entity)
        {
            return new DownloadHistoryViewModel()
            {
                FileId = entity.FileId,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                UserEmail = entity.UserEmail,
                UserName = entity.UserName,
                UserPhone = entity.UserPhone,
                Date = DateTime.Parse(entity.Date),
            };
        }

        public static DownloadHistory ToEntity(this DownloadDataViewModel viewModel, LocalFile localFile)
        {
            return new DownloadHistory()
            {
                FileId = localFile.Id,
                FileName = localFile.Name,
                FilePath = localFile.Path,
                UserEmail = viewModel.Email,
                UserName = viewModel.Name,
                UserPhone = viewModel.Phone,
                Date = DateTime.Now.ToString(),
            };
        }
    }
}
