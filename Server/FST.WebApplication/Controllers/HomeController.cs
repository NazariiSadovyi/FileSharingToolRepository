using FST.DataAccess.Repositories.Interfaces;
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
            var fullPath = Path.Combine(localFile.Path, localFile.Name);
            fullPath = fullPath.Replace(@"C:\", @"\test\");
            fullPath = fullPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var viewModel = new PreviewFileViewModel()
            {
                Name = localFile.Name,
                FullPath = fullPath
            };

            return View("PhotoPreviewView", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
