using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class HeroRepository : IHeroRepository
    {
        private readonly AppDbContext _context;
        public HeroRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddHeroAsync(Hero hero)
        {
            await _context.Heroes.AddAsync(hero);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteHeroAsync(int id)
        {
           var hero = await _context.Heroes.FindAsync(id);
              if (hero != null)
              {
                _context.Heroes.Remove(hero);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Hero>> GetAllHeroesAsync()
        {
           return await _context.Heroes.ToListAsync();
        }

        public async Task<Hero> GetHeroByIdAsync(int id)
        {
            return await _context.Heroes.FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task UpdateHeroAsync(Hero hero)
        {
            await _context.Heroes.FindAsync(hero.Id);
            _context.Heroes.Update(hero);
            await _context.SaveChangesAsync();

        }
    }
}
