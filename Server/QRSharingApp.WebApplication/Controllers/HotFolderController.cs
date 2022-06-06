using Microsoft.AspNetCore.Mvc;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using QRSharingApp.WebApplication.Converters;
using System;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotFolderController : ControllerBase
    {
        private readonly IHotFolderRepository _hotFolderRepository;

        public HotFolderController(IHotFolderRepository hotFolderRepository)
        {
            _hotFolderRepository = hotFolderRepository;
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
            var decodedPath = Uri.EscapeDataString(folderPath);
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
