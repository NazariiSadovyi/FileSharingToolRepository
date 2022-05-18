using Microsoft.AspNetCore.Mvc;
using QRSharingApp.Contract;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingRepository _settingRepository;

        public SettingController(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpGet("{settingKey}")]
        public async Task<IActionResult> Get(string settingKey)
        {
            var value = await _settingRepository.GetStringSettingAsync(settingKey);
            
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateSettingContract updateContract)
        {
            await _settingRepository.SetSettingAsync(updateContract.Key, updateContract.Value);

            return Ok();
        }
    }
}
