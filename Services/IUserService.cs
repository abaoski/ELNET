using System.Collections.Generic;
using System.Threading.Tasks;
using SubdivisionManagement.Models;

namespace SubdivisionManagement.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AuthenticateAsync(string email, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<int> GetUserCountAsync();
        Task<List<User>> GetRecentUsersAsync(int count);
        Task<int> GetUserCountByRoleAsync(UserRoleType role);
        Task<User> GetUserByIdAsync(int id);
    }
} 