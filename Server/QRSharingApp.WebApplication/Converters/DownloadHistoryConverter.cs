using QRSharingApp.Contract;
using QRSharingApp.DataAccess.Entities;
using QRSharingApp.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static List<DownloadHistoryContract> ToContracts(this IEnumerable<DownloadHistory> entities)
        {
            return entities.Select(ToContract).ToList();
        }

        public static DownloadHistoryContract ToContract(this DownloadHistory entity)
        {
            if (entity == null)
                return null;

            return new DownloadHistoryContract()
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Date = entity.Date,
                FileId = entity.FileId,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                UserEmail = entity.UserEmail,
                UserPhone = entity.UserPhone,
            };
        }
    }
}
