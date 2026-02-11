using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
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
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SettingsDto>> CreateSettings([FromBody] CreateSettingsDto createSettingsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email is unique
            var settings = await _settingsService.CreateSettingsAsync(createSettingsDto);
            return CreatedAtAction(nameof(GetSettingsById), new { id = settings.Id }, settings);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SettingsDto>> GetSettingsById(int id)
        {
            var settings = await _settingsService.GetSettingsByIdAsync(id);
            if (settings == null)
            {
                return NotFound($"Settings with ID {id} not found.");
            }
            return Ok(settings);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SettingsDto>> UpdateSettings(int id, [FromBody] UpdateSettingsDto updateSettingsDto)
        {
            if (id != updateSettingsDto.Id)
            {
                return BadRequest("Settings ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var settings = await _settingsService.UpdateSettingsAsync(updateSettingsDto);
            if (settings == null)
            {
                return NotFound($"Settings with ID {id} not found.");
            }
            return Ok(settings);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSettings(int id)
        {
            var result = await _settingsService.DeleteSettingsAsync(id);
            if (!result)
            {
                return NotFound($"Settings with ID {id} not found.");
            }
            return NoContent();
        }
        [HttpPut("social-media")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateSocialMediaLinks([FromBody] Dictionary<string, string> socialLinks)
        {
            if (socialLinks == null)
            {
                return BadRequest("Social media links cannot be null.");
            }

            var result = await _settingsService.UpdateSocialMediaLinksAsync(socialLinks);
            return Ok(result);
        }
        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateSettings([FromBody] SettingsDto settings)
        {
            if (settings == null)
            {
                return BadRequest("Settings cannot be null.");
            }

            var isValid = await _settingsService.ValidateSettingsAsync(settings);
            return Ok(new { isValid });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetAllSettings()
        {
            var settings = await _settingsService.GetAllSettingsAsync();
            return Ok(settings);
        }
        [HttpGet("social-media")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, string>>> GetSocialMediaLinks()
        {
            var socialLinks = await _settingsService.GetSocialMediaLinksAsync();
            return Ok(socialLinks);
        }
    } 
}
