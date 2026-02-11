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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetAllSettings()
        {
            var settings = await _settingsService .GetAllSettingsAsync();
            return Ok(settings);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SettingsDto>> GetSettingsById(int id)
        {
            var settings = await _settingsService .GetSettingsByIdAsync(id);
            if (settings == null)
            {
                return NotFound($"Settings with ID {id} not found.");
            }
            return Ok(settings);
        }
        //[HttpGet("default")]
        //public async Task<ActionResult<SettingsDto>> GetDefaultSettings()
        //{
        //    var settings = await _settingsService .GetDefaultSettingsAsync();
        //    if (settings == null)
        //    {
        //        return NotFound("No default settings found.");
        //    }
        //    return Ok(settings);
        //}
        //[HttpPut("current")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<bool>> UpdateCurrentSettings([FromBody] UpdateSettingsDto updateSettingsDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var result = await _settingsService .UpdateCurrentSettingsAsync(updateSettingsDto);
        //        if (!result)
        //        {
        //            return NotFound("Current settings not found.");
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
        [HttpGet("social-media")]
        public async Task<ActionResult<Dictionary<string, string>>> GetSocialMediaLinks()
        {
            var socialLinks = await _settingsService .GetSocialMediaLinksAsync();
            return Ok(socialLinks);
        }
    }
    //public async Task<ActionResult<bool>> CheckEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
    //{
    //    if (string.IsNullOrEmpty(email))
    //    {
    //        return BadRequest("Email cannot be empty.");
    //    }

    //    bool isUnique;


    //    return Ok(new { email, isUnique });
    //}
    //[HttpPost("reset-to-default")]
    //[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<bool>> ResetToDefaultSettings()
    //{
    //    try
    //    {
    //        var result = await _settingsService .ResetToDefaultSettingsAsync();
    //        return Ok(new { success = result, message = result ? "Settings reset to default successfully." : "Failed to reset settings." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}
    //[HttpPost("backup")]
    //[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<bool>> BackupCurrentSettings()
    //{
    //    try
    //    {
    //        var result = await _settingsService .BackupCurrentSettingsAsync();
    //        return Ok(new { success = result, message = result ? "Settings backed up successfully." : "Failed to backup settings." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}
    //[HttpPost("restore-from-backup")]
    //[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<bool>> RestoreSettingsFromBackup()
    //{
    //    try
    //    {
    //        var result = await _settingsService .RestoreSettingsFromBackupAsync();
    //        return Ok(new { success = result, message = result ? "Settings restored from backup successfully." : "Failed to restore settings from backup." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

}