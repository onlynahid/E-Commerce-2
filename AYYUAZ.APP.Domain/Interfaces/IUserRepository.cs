using AYYUAZ.APP.Domain.Entities;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmail(string email);
        Task<User?> GetByUsername(string username);
        Task<User?> GetById(string id); 
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id); 
        Task<IEnumerable<User>> GetAll();
        Task<int> GetUserCount();
    }
}