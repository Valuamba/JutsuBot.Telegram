using CliverBot.Console.DataAccess;
using FormBot.WebApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace JutsuBot.Elements.DataAccess.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User UserState { get; set; }

        public Task ClearStage()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCachedData(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByIdAsync(long userId)
        {
            return await _context.Users
               //.Include(u => u.HandledMessages)
               .SingleOrDefaultAsync(i => !i.IsBotStopped && i.Id == userId);
        }

        public async Task<List<User>> GetUsersNotAdmins()
        {
            return await _context.Users
                //.Include(u => u.HandledMessages)
                .Where(u => !u.IsBotStopped && u.Role != Role.Admin).ToListAsync();
        }

        public async Task<List<User>> GetUsersWithPositionLevel(Role role)
        {
            return await _context.Users
                //.Include(u => u.HandledMessages)
                .Where(u => !u.IsBotStopped && u.Role == role).ToListAsync();
        }

        public Task UpdateCachedData(int userId, string stagedModel)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUser(User User)
        {
            _context.Update(User);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUsers(List<User> Users)
        {
            _context.UpdateRange(Users);
        }
    }
}
