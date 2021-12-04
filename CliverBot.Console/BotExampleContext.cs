using CliverBot.Console.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console
{
    public class BotExampleContext : BaseUpdateContext
    {
        public ElementClient<BaseBot> ElementClient { get; set; }
        public UserPrivateInfo Info { get; set; }
    }

    public class UserPrivateInfo
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}
