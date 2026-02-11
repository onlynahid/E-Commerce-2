using AYYUAZ.APP.Domain.Entities;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(string id); // Changed to string to match Identity
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id); // Changed to string to match Identity
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> GetUserCountAsync();
    }
}