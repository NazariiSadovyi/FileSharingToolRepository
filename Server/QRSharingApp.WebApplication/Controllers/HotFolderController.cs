using Microsoft.AspNetCore.Mvc;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using QRSharingApp.WebApplication.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotFolderController : ControllerBase
    {
        private readonly IHotFolderRepository _hotFolderRepository;
        private readonly ILocalFileRepository _localFileRepository;
        private readonly IFileHubService _fileHubService;

        public HotFolderController(
            IHotFolderRepository hotFolderRepository,
            ILocalFileRepository localFileRepository,
            IFileHubService fileHubService)
        {
            _hotFolderRepository = hotFolderRepository;
            _localFileRepository = localFileRepository;
            _fileHubService = fileHubService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var entities = await _hotFolderRepository.GetAll();
            var contracts = entities.ToContracts();

            return Ok(contracts);
        }

        [HttpGet("{folderPath}")]
        public async Task<IActionResult> Get(string folderPath)
        {
            var decodedPath = HttpUtility.UrlDecode(folderPath);
            var entity = await _hotFolderRepository.GetByPath(decodedPath);
            var contract = entity.ToContract();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string folderPath)
        {
            var entity = await _hotFolderRepository.Add(folderPath);
            var contract = entity.ToContract();

            return Ok(contract);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _hotFolderRepository.GetById(id);
            var contract = entity.ToContract();
            await _hotFolderRepository.Remove(id);

            return Ok(contract);
        }
    }
}
