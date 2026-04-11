using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService  _settingsService ;
        public SettingsController(ISettingsService  settingsService )
        {
            _settingsService  = settingsService ;
        }
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetAllSettings()
        {
            var settings = await _settingsService.GetAllSettings();
            return Ok(settings);
        }
        [HttpGet("get-by-id{id}")]
        public async Task<ActionResult<SettingsDto>> GetSettingsById(int id)
        {
            var settings = await _settingsService.GetSettingsById(id);
            return Ok(settings);
        }
        [HttpGet("get-social-media")]
        public async Task<ActionResult<Dictionary<string, string>>> GetSocialMediaLinks()
        {
            var socialLinks = await _settingsService.GetSocialMediaLinks();
            return Ok(socialLinks);
        }
    }

}