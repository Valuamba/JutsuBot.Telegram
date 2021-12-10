using CliverBot.Console.DataAccess;
using FormBot.WebApi.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JutsuBot.Elements.DataAccess.Repositories
{
    public class TrackedMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public TrackedMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrackedMessage> AddMessage(TrackedMessage message)
        {
            var entityMessage = await _context.TrackedMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return entityMessage.Entity;
        }

        public async Task DeleteMessages(long chatId, MessageType messageType)
        {
            var messagesToBeDeleted = _context.TrackedMessages.Where(m => m.ChatId == chatId && m.MessageType == messageType);
            _context.TrackedMessages.RemoveRange(messagesToBeDeleted);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessages(List<TrackedMessage> trackedMessages)
        {
            await _context.TrackedMessages.AddRangeAsync(trackedMessages);
        }
    }
}
