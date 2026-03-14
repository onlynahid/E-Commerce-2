using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<User>();
        }

        public Task<User?> GetByEmail(string email)
        {
            return _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByUsername(string username)
        {
            return _dbSet.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public Task<User?> GetById(string id)
        {
            return _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _dbSet.AnyAsync(u => u.UserName == username);
        }

        public async Task<User> AddAsync(User user)
        {
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _dbSet.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await GetById(id);
            if (user == null) return false;

            _dbSet.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public Task<int> GetUserCount()
        {
            return _dbSet.CountAsync();
        }
    }
}