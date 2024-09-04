using System;
using Persistence.Models;

namespace Persistence.Repositories
{
    public interface IUserTokenRepository
    {
        Task<UserToken?> GetUserTokenAsync(string userId);
        Task AddUserTokenAsync(UserToken userToken);
        Task UpdateUserTokenAsync(UserToken userToken);
        Task RemoveUserTokenAsync(string userId);
    }
}
