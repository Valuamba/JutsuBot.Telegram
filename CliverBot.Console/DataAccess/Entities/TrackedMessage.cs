using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess
{
    public class TrackedMessage
    {
        public int MessageId { get; set; }
        public long ChatId { get; set; }
        public MessageType MessageType { get; set; }
        public string? MessageText { get; set; }
    }
}
