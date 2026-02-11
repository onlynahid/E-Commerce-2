using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HeroDto>>> GetAllHeroes()
        {
            var heroes = await _heroService.GetAllHeroesAsync();
            return Ok(heroes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<HeroDto>> GetHeroById(int id)
        {
            var hero = await _heroService.GetHeroByIdAsync(id);
            if (hero == null)
            {
                return NotFound($"Hero with ID {id} not found.");
            }
            return Ok(hero);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HeroDto>> CreateHero([FromForm] CreateHeroDto createHeroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hero = await _heroService.AddHeroAsync(createHeroDto);
            return CreatedAtAction(nameof(GetHeroById), new { id = hero.Id }, hero);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HeroDto>> UpdateHero(int id, [FromForm] UpdateHeroDto updateHeroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hero = await _heroService.UpdateHeroAsync(id, updateHeroDto);
            if (hero == null)
            {
                return NotFound($"Hero with ID {id} not found.");
            }
            return Ok(hero);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteHero(int id)
        {
            var result = await _heroService.DeleteHeroAsync(id);
            if (!result)
            {
                return NotFound($"Hero with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
