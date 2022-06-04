using Microsoft.AspNetCore.Mvc;
using QRSharingApp.Contract.LocalFile;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using QRSharingApp.WebApplication.Services;
using System.Threading.Tasks;
using System.Web;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalFileController : ControllerBase
    {
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IFileHubService _fileHubService;

        public LocalFileController(ILocalFileRepository localFileRepository,
            IFileHubService fileHubService)
        {
            _localFileRepository = localFileRepository;
            _fileHubService = fileHubService;
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
            var decodedPath = HttpUtility.UrlDecode(filePath);
            var localFile = await _localFileRepository.GetByFullPath(decodedPath);
            var contract = localFile.ToContract();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateLocalFile createLocalFile)
        {
            var localFile = await _localFileRepository.Add(createLocalFile.Path);
            var contract = localFile.ToContract();
            var viewModel = LocalFileConverter.ComposeFilePreviewViewModel(localFile, this);

            await _fileHubService.SendFileAddedAsync(viewModel);

            return Ok(contract);
        }

        [HttpDelete("{filePath}")]
        public async Task<IActionResult> Delete(string filePath)
        {
            var decodedPath = HttpUtility.UrlDecode(filePath);
            var localFile = await _localFileRepository.GetByFullPath(decodedPath);

            await _localFileRepository.Remove(localFile.Id);
            await _fileHubService.SendFileRemovedAsync(localFile.Id);

            return Ok();
        }
    }
}
