using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess
{
    public class MessageRepository
    {
        public List<TrackedMessage> TrackedMessages { get; set; } = new();

        public void AddMessage(TrackedMessage message)
        {
            TrackedMessages.Add(message);
        }

        public void DeleteMessages(long chatId, MessageType messageType)
        {
            var messagesToBeDeleted = TrackedMessages.Where(m => m.ChatId == chatId);
            TrackedMessages.RemoveAll(m => m.ChatId == chatId && m.MessageType == messageType);
        }
    }
}
