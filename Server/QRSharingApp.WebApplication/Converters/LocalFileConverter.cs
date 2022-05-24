using Microsoft.AspNetCore.Mvc;
using QRSharingApp.Contract.LocalFile;
using QRSharingApp.DataAccess.Entities;
using QRSharingApp.WebApplication.Controllers;
using QRSharingApp.WebApplication.Helpers;
using QRSharingApp.WebApplication.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QRSharingApp.WebApplication.Converters
{
    public static class LocalFileConverter
    {
        public static List<LocalFileContract> ToContracts(this IEnumerable<LocalFile> localFiles)
        {
            return localFiles.Select(ToContract).ToList();
        }

        public static LocalFileContract ToContract(this LocalFile localFile)
        {
            if (localFile == null)
                return null;

            return new LocalFileContract()
            {
                Id = localFile.Id,
                Name = localFile.Name,
                Path = localFile.Path
            };
        }

        public static FilePreviewViewModel ComposeFilePreviewViewModel(LocalFile localFile, ControllerBase controller)
        {
            var extension = Path.GetExtension(localFile.Name);
            var idWithExtension = localFile.Id + extension;
            var viewModel = new FilePreviewViewModel()
            {
                Id = localFile.Id,
                Name = localFile.Name,
                Extension = extension.Replace(".", ""),
                Adress = controller.Url.Action(nameof(FileController.PhysicalFile), "File", new { idWithExtension }),
                QRCodeAdress = controller.Url.Action(nameof(FileController.QRCode), "File", new { localFile.Id }),
                ThumbnailAdress = controller.Url.Action(nameof(FileController.Thumbnail), "File", new { idWithExtension })
            };

            if (FileNameHelper.IsPhoto(localFile.Name))
            {
                viewModel.IsPhoto = true;
            }
            else if (FileNameHelper.IsVideo(localFile.Name))
            {
                viewModel.IsVideo = true;
            }

            return viewModel;
        }
    }
}
