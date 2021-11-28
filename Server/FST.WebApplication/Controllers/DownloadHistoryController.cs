using FST.DataAccess.Repositories.Interfaces;
using FST.WebApplication.Converters;
using FST.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FST.WebApplication.Controllers
{
    public class DownloadHistoryController : Controller
    {
        private readonly ILogger<DownloadHistoryController> _logger;
        private readonly IDownloadHistoryRepository _downloadHistoryRepository;
        private readonly ILocalFileRepository _localFileRepository;

        public DownloadHistoryController(ILogger<DownloadHistoryController> logger,
            IDownloadHistoryRepository downloadHistoryRepository,
            ILocalFileRepository localFileRepository)
        {
            _downloadHistoryRepository = downloadHistoryRepository;
            _localFileRepository = localFileRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var downloadHistory = await _downloadHistoryRepository.GetAll();
            var viewModels = downloadHistory.Select(_ => _.ToViewModel());

            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Save(DownloadDataViewModel model)
        {
            var localFile = await _localFileRepository.GetById(model.Id);
            var downloadHistory = model.ToEntity(localFile);
            await _downloadHistoryRepository.Add(downloadHistory);

            return RedirectToAction(nameof(FileController.Preview), "File", new { model.Id });
        }
    }
}
