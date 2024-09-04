using System;
using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly ApplicationDBContext _context;

        public UserTokenRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<UserToken?> GetUserTokenAsync(string userId)
        {
            return await _context.UserToken
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.LoginProvider == "custom"  && ut.Name == "AccessToken");
        }

        public async Task AddUserTokenAsync(UserToken userToken)
        {
            await _context.UserToken.AddAsync(userToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserTokenAsync(UserToken userToken)
        {
            _context.UserToken.Update(userToken);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserTokenAsync(string userId)
        {
            var userToken = await GetUserTokenAsync(userId);
            if (userToken != null)
            {
                _context.UserToken.Remove(userToken);
                await _context.SaveChangesAsync();
            }
        }
    }
}
