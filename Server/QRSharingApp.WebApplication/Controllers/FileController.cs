using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRSharingApp.Common.Enums;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Common.Settings.Interfaces;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using QRSharingApp.WebApplication.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication.Controllers
{
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IDownloadHistoryRepository _downloadHistoryRepository;
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly IFileThumbnailService _fileThumbnailService;
        private readonly IHotFolderRepository _hotFolderRepository;
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebServerService _webServerService;
        private readonly IWebSetting _webSetting;
        private readonly IWifiSetting _wifiSetting;
        private readonly IWifiService _wifiService;

        public FileController(
            ILogger<FileController> logger,
            IDownloadHistoryRepository downloadHistoryRepository,
            IQRCodeGeneratorService qrCodeGeneratorService,
            IFileThumbnailService fileThumbnailService,
            ILocalFileRepository localFileRepository,
            IWebServerService webServerService,
            IWebHostEnvironment webHostEnvironment,
            IHotFolderRepository hotFolderRepository,
            IWebSetting webSetting,
            IWifiSetting wifiSetting,
            IWifiService wifiService)
        {
            _downloadHistoryRepository = downloadHistoryRepository;
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _fileThumbnailService = fileThumbnailService;
            _hotFolderRepository = hotFolderRepository;
            _localFileRepository = localFileRepository;
            _webHostEnvironment = webHostEnvironment;
            _webServerService = webServerService;
            _wifiSetting = wifiSetting;
            _wifiService = wifiService;
            _webSetting = webSetting;
            _logger = logger;

            InitBackgroundImage();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = new FilesGalleryViewModel();

            var hotFolderPathes = (await _hotFolderRepository.GetAll()).Select(_ => _.FolderPath).ToList();
            var localFiles = await _localFileRepository.GetAll();
            foreach (var localFile in localFiles.OrderByDescending(_ => DateTime.Parse(_.AddedDate, CultureInfo.InvariantCulture)))
            {
                var fileExist = System.IO.File.Exists(Path.Combine(localFile.Path, localFile.Name));
                if (fileExist && hotFolderPathes.Contains(localFile.Path))
                {
                    result.FilePreviews.Add(LocalFileConverter.ComposeFilePreviewViewModel(localFile, this));
                }
            }

            var urlQrCodeData = _qrCodeGeneratorService.Base64Image(_webServerService.WebUrl);
            result.GalleryUrlQRImageData = urlQrCodeData;

            var wifiConfigString = _wifiService.GenerateConfigString(
                _wifiSetting.WifiLogin,
                (WifiAuthenticationType)_wifiSetting.WifiAuthenticationType,
                _wifiSetting.WifiPassword,
                _wifiSetting.WifiIsHidden);
            result.WifiQRImageData = _qrCodeGeneratorService.Base64Image(wifiConfigString);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Preview(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = LocalFileConverter.ComposeFilePreviewViewModel(localFile, this);
            viewModel.DownloadViaForm = _webSetting.DownloadViaForm;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> PhysicalFile(string idWithExtension)
        {
            var id = Path.GetFileNameWithoutExtension(idWithExtension);
            var localFile = await _localFileRepository.GetById(id);
            if (localFile == null)
            {
                return Ok();
            }

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
        public async Task<IActionResult> Thumbnail(string idWithExtension)
        {
            var id = Path.GetFileNameWithoutExtension(idWithExtension);
            var localFile = await _localFileRepository.GetById(id);
            if (localFile == null)
            {
                return Ok();
            }

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

        private void InitBackgroundImage()
        {
            var localImagePath = _webSetting.WebBackgroundImagePath;
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
