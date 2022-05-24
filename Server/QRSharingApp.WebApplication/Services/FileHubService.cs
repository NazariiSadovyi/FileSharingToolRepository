using Microsoft.AspNetCore.SignalR;
using QRSharingApp.WebApplication.Models;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication.Services
{
    public interface IFileHubService
    {
        Task SendFileAddedAsync(FilePreviewViewModel viewModel);
    }

    public class FileHubService : IFileHubService
    {
        private readonly IHubContext<FileHub> _fileHub;

        public FileHubService(IHubContext<FileHub> fileHub)
        {
            _fileHub = fileHub;
        }

        public async Task SendFileAddedAsync(FilePreviewViewModel viewModel)
        {
            await _fileHub.Clients.All.SendAsync(
                "FileAdded",
                viewModel.Id,
                viewModel.Name,
                viewModel.Adress,
                viewModel.ThumbnailAdress,
                viewModel.QRCodeAdress,
                viewModel.Extension,
                viewModel.IsVideo);
        }
    }
}
