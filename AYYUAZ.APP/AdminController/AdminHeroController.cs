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
    public class AdminHeroController : ControllerBase
    {
        private readonly IHeroService _heroService;
        public AdminHeroController(IHeroService heroService)
        {
            _heroService = heroService;
        }
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<HeroDto>>> GetAllHeroes()
        {
            var heroes = await _heroService.GetAllHeroesAsync();
            return Ok(heroes);
        }
        [HttpGet("get-by-id{id}")]
        public async Task<ActionResult<HeroDto>> GetHeroById(int id)
        {
            var hero = await _heroService.GetHeroByIdAsync(id);
            return Ok(hero);
        }
        [HttpPost("create-hero")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<HeroDto>> CreateHero([FromForm] CreateHeroDto createHeroDto)
        {
            var hero = await _heroService.AddHeroAsync(createHeroDto);
            return CreatedAtAction(nameof(GetHeroById), new { id = hero.Id }, hero);
        }
        [HttpPut("update-hero{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<HeroDto>> UpdateHero(int id, [FromForm] UpdateHeroDto updateHeroDto)
        {
            var hero = await _heroService.UpdateHeroAsync(id, updateHeroDto);
            return Ok(hero);
        }
        [HttpDelete("delete-hero{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteHero(int id)
        {
            var result = await _heroService.DeleteHeroAsync(id);
            return NoContent();
        }
    }
}
