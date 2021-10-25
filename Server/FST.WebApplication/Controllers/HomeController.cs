using FST.DataAccess.Repositories.Interfaces;
using FST.WebApplication.Helpers;
using FST.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FST.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILocalFileRepository _localFileRepository;

        public HomeController(ILogger<HomeController> logger,
            ILocalFileRepository localFileRepository)
        {
            _logger = logger;
            _localFileRepository = localFileRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> File(string id)
        {
            var localFile = await _localFileRepository.GetById(id);
            var viewModel = new FilePreviewViewModel()
            {
                Name = localFile.Name,
            };

            if (FileNameHelper.IsPhoto(localFile.Name))
            {
                viewModel.Adress = Url.Action("image", new { id });
                viewModel.IsPhoto = true;
            }
            else if (FileNameHelper.IsVideo(localFile.Name))
            {
                viewModel.Adress = Url.Action("video", new { id });
                viewModel.IsVideo = true;
            }
            else
            {
                throw new NotImplementedException();
            }

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
