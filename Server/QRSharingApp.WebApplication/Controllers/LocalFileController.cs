using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRSharingApp.Contract.LocalFile;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using QRSharingApp.WebApplication.Services;
using System;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalFileController : ControllerBase
    {
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IFileHubService _fileHubService;
        private readonly ILogger<LocalFileController> _logger;

        public LocalFileController(
            ILocalFileRepository localFileRepository,
            IFileHubService fileHubService,
            ILogger<LocalFileController> logger)
        {
            _localFileRepository = localFileRepository;
            _fileHubService = fileHubService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var files = await _localFileRepository.GetAll();
            var contracts = files.ToContracts();

            return Ok(contracts);
        }

        [HttpGet("{filePath}")]
        public async Task<IActionResult> Get(string filePath)
        {
            var decodedPath = Uri.UnescapeDataString(filePath);
            var localFile = await _localFileRepository.GetByFullPath(decodedPath);
            var contract = localFile.ToContract();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateLocalFile createLocalFile)
        {
            _logger.LogInformation($"New file added: {createLocalFile.Path}");
            var localFile = await _localFileRepository.Add(createLocalFile.Path);
            var contract = localFile.ToContract();
            var viewModel = LocalFileConverter.ComposeFilePreviewViewModel(localFile, this);

            await _fileHubService.SendFileAddedAsync(viewModel);

            return Ok(contract);
        }

        [HttpDelete("{filePath}")]
        public async Task<IActionResult> Delete(string filePath)
        {
            var decodedPath = Uri.UnescapeDataString(filePath);
            var localFile = await _localFileRepository.GetByFullPath(decodedPath);

            await _localFileRepository.Remove(localFile.Id);
            await _fileHubService.SendFileRemovedAsync(localFile.Id);

            return Ok();
        }
    }
}
