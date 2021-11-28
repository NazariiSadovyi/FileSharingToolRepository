﻿using FST.Common.Services.Interfaces;
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
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly IFileThumbnailService _fileThumbnailService;
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IWebServerService _webServerService;

        public FileController(ILogger<FileController> logger,
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = new List<FilePreviewViewModel>();

            var localFiles = await _localFileRepository.GetAll();
            foreach (var localFile in localFiles)
            {
                result.Add(ComposeFilePreviewViewModel(localFile));
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Preview(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = ComposeFilePreviewViewModel(localFile);

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

        [HttpPost]
        public IActionResult Download(DownloadDataViewModel model)
        {
            return RedirectToAction(nameof(Preview), new { model.Id });
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
    }
}