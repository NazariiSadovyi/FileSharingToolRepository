using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.DataAccess.Entities;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using QRSharingApp.WebApplication.Helpers;
using QRSharingApp.WebApplication.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace QRSharingApp.WebApplication.Controllers
{
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IDownloadHistoryRepository _downloadHistoryRepository;
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly IFileThumbnailService _fileThumbnailService;
        private readonly ISharedSettingService _sharedSettingService;
        private readonly IHotFolderRepository _hotFolderRepository;
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebServerService _webServerService;

        public FileController(
            ILogger<FileController> logger,
            IDownloadHistoryRepository downloadHistoryRepository,
            IQRCodeGeneratorService qrCodeGeneratorService,
            IFileThumbnailService fileThumbnailService,
            ISharedSettingService sharedSettingService,
            ILocalFileRepository localFileRepository,
            IWebServerService webServerService,
            IWebHostEnvironment webHostEnvironment,
            IHotFolderRepository hotFolderRepository)
        {
            _downloadHistoryRepository = downloadHistoryRepository;
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _sharedSettingService = sharedSettingService;
            _fileThumbnailService = fileThumbnailService;
            _hotFolderRepository = hotFolderRepository;
            _localFileRepository = localFileRepository;
            _webHostEnvironment = webHostEnvironment;
            _webServerService = webServerService;
            _logger = logger;

            InitBackgroundImage();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = new List<FilePreviewViewModel>();

            var hotFolderPathes = (await _hotFolderRepository.GetAll()).Select(_ => _.FolderPath).ToList();
            var localFiles = await _localFileRepository.GetAll();
            foreach (var localFile in localFiles)
            {
                var fileExist = System.IO.File.Exists(Path.Combine(localFile.Path, localFile.Name));
                if (fileExist && hotFolderPathes.Contains(localFile.Path))
                {
                    result.Add(ComposeFilePreviewViewModel(localFile));
                }
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Preview(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = ComposeFilePreviewViewModel(localFile);
            viewModel.DownloadViaForm = _sharedSettingService.DownloadViaForm;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> PhysicalFile(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);

            return PhysicalFile(fullPath, "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpGet]
        public async Task<IActionResult> PhysicalFileWithDataSave([FromQuery]string id, [FromQuery] string name, [FromQuery] string email, [FromQuery] string phone)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            var downloadHistory = new DownloadDataViewModel() 
            {
                Id = id,
                Email = email,
                Name = name,
                Phone = phone
            }.ToEntity(localFile);
            await _downloadHistoryRepository.Add(downloadHistory);

            return PhysicalFile(fullPath, "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpGet]
        public async Task<IActionResult> Thumbnail(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            var fileThumbnailStream = new MemoryStream();
            await _fileThumbnailService.SaveToStreamAsync(fullPath, fileThumbnailStream);

            return File(fileThumbnailStream, "image/png");
        }

        [HttpGet]
        public IActionResult QRCode(string id)
        {
            var fileWebPath = _webServerService.GetFilePath(id);
            var qrCodeStream = new MemoryStream();
            _qrCodeGeneratorService.SaveToStream(fileWebPath, qrCodeStream);

            return File(qrCodeStream, "image/jpeg");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = new DownloadDataViewModel()
            {
                Id = localFile.Id,
                Name = localFile.Name,
                PhysicalFileAdress = Url.Action(nameof(PhysicalFile), new { localFile.Id })
            };

            return View(viewModel);
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
                Id = localFile.Id,
                Name = localFile.Name,
                Adress = Url.Action(nameof(PhysicalFile), new { localFile.Id }),
                QRCodeAdress = Url.Action(nameof(QRCode), new { localFile.Id }),
                ThumbnailAdress = Url.Action(nameof(Thumbnail), new { localFile.Id })
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

        private void InitBackgroundImage()
        {
            var localImagePath = _sharedSettingService.WebBackgroundImagePath;
            if (string.IsNullOrEmpty(localImagePath) || !System.IO.File.Exists(localImagePath))
            {
                return;
            }

            var imageName = Path.GetFileName(localImagePath);
            var webImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageName);
            if (System.IO.File.Exists(webImagePath))
            {
                return;
            }

            System.IO.File.Copy(localImagePath, webImagePath);
        }
    }
}
