using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AYYUAZ.APP.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;
        public AdminSettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        [HttpPost("create-settings")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<SettingsDto>> CreateSettings([FromBody] CreateSettingsDto createSettingsDto)
        {  
            var settings = await _settingsService.CreateSettingsAsync(createSettingsDto);
            return CreatedAtAction(nameof(GetSettingsById), new { id = settings.Id }, settings);
        }
        [HttpGet("get-by-id{id}")]
        public async Task<ActionResult<SettingsDto>> GetSettingsById(int id)
        {
            var settings = await _settingsService.GetSettingsById(id);
            return Ok(settings);
        }
        [HttpPut("update-settings{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<SettingsDto>> UpdateSettings(int id, [FromBody] UpdateSettingsDto updateSettingsDto)
        {
            if (id != updateSettingsDto.Id)
            {
                return BadRequest("Settings ID mismatch.");
            }
            var settings = await _settingsService.UpdateSettingsAsync(updateSettingsDto);
            return Ok(settings);
        }
        [HttpDelete("delete-settings{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteSettings(int id)
        {
            var result = await _settingsService.DeleteSettingsAsync(id);
            return NoContent();
        }
        [HttpPut("social-media")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateSocialMediaLinks([FromBody] Dictionary<string, string> socialLinks)
        {

            var result = await _settingsService.UpdateSocialMediaLinksAsync(socialLinks);
            return Ok(result);
        }
        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateSettings([FromBody] SettingsDto settings)
        {

            var isValid = await _settingsService.ValidateSettingsAsync(settings);
            return Ok(new { isValid });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetAllSettings()
        {
            var settings = await _settingsService.GetAllSettings();
            return Ok(settings);
        }
        [HttpGet("social-media")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, string>>> GetSocialMediaLinks()
        {
            var socialLinks = await _settingsService.GetSocialMediaLinks();
            return Ok(socialLinks);
        }
    } 
}
