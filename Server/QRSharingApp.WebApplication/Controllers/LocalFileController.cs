using Microsoft.AspNetCore.Mvc;
using QRSharingApp.Contract.LocalFile;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using System.Threading.Tasks;
using System.Web;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalFileController : ControllerBase
    {
        private readonly ILocalFileRepository _localFileRepository;

        public LocalFileController(ILocalFileRepository localFileRepository)
        {
            _localFileRepository = localFileRepository;
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
        public async Task<IActionResult> Post([FromBody] CreateLocalFiles filePathes)
        {
            var localFiles = await _localFileRepository.Add(filePathes.Pathes);
            var contracts = localFiles.ToContracts();

            return Ok(contracts);
        }
    }
}
