using FST.Common.Services.Interfaces;
using FST.DataAccess.Entities;
using FST.DataAccess.Repositories.Interfaces;
using FST.WebApplication.Helpers;
using FST.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FST.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly IFileThumbnailService _fileThumbnailService;
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IWebServerService _webServerService;

        public HomeController(ILogger<HomeController> logger,
            IQRCodeGeneratorService qrCodeGeneratorService,
            IFileThumbnailService fileThumbnailService,
            ILocalFileRepository localFileRepository,
            IWebServerService webServerService)
        {
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _fileThumbnailService = fileThumbnailService;
            _localFileRepository = localFileRepository;
            _webServerService = webServerService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = new List<FilePreviewViewModel>();

            var localFiles = await _localFileRepository.GetAll();
            foreach (var localFile in localFiles)
            {
                var viewModel = ComposeFilePreviewViewModel(localFile);
                viewModel.QRCodeAdress = Url.Action(nameof(QRCode), new { localFile.Id });
                viewModel.ThumbnailAdress = Url.Action(nameof(FileThumbnail), new { localFile.Id });
                result.Add(viewModel);
            }

            return View(result);
        }

        public async Task<IActionResult> File(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = ComposeFilePreviewViewModel(localFile);

            return View("FilePreviewView", viewModel);
        }

        public async Task<IActionResult> Video(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            return PhysicalFile(fullPath, "application/octet-stream", enableRangeProcessing: true);
        }

        public async Task<IActionResult> Image(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            return PhysicalFile(fullPath, "application/octet-stream", enableRangeProcessing: true);
        }

        public async Task<IActionResult> FileThumbnail(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            var fileThumbnailStream = new MemoryStream();
            await _fileThumbnailService.SaveToStreamAsync(fullPath, fileThumbnailStream);
            return File(fileThumbnailStream, "image/png");
        }

        public IActionResult QRCode(string id)
        {
            var fileWebPath = _webServerService.GetFilePath(id);
            var qrCodeStream = new MemoryStream();
            _qrCodeGeneratorService.SaveToStream(fileWebPath, qrCodeStream);
            return File(qrCodeStream, "image/jpeg");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private FilePreviewViewModel ComposeFilePreviewViewModel(LocalFile localFile)
        {
            var viewModel = new FilePreviewViewModel()
            {
                Name = localFile.Name,
            };

            if (FileNameHelper.IsPhoto(localFile.Name))
            {
                viewModel.Adress = Url.Action(nameof(Image), new { localFile.Id });
                viewModel.IsPhoto = true;
            }
            else if (FileNameHelper.IsVideo(localFile.Name))
            {
                viewModel.Adress = Url.Action(nameof(Video), new { localFile.Id });
                viewModel.IsVideo = true;
            }

            return viewModel;
        }
    }
}
