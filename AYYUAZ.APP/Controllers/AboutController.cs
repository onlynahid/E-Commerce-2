using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;
        private readonly ILogger<AboutController> _logger;
        public AboutController(IAboutService aboutService, ILogger<AboutController> logger)
        {
            _aboutService = aboutService;
            _logger = logger;
         
        }
        [HttpGet("get-all")]
        public async  Task<ActionResult<IEnumerable<AboutDto>>> GetAllAbout()
        {
            var aboutList = await _aboutService.GetAllAbout();
            return Ok(aboutList);
        }
        [HttpGet("get-by-id{id}")]
        public async Task<ActionResult<AboutDto>> GetAboutById(int id)
        {
            var about = await _aboutService.GetAboutById(id);
            return Ok(about);
        }
       

    }
}