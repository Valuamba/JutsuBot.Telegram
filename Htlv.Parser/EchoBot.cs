using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console
{
    public class EchoBot : BaseBot
    {
        public EchoBot(IOptions<BotSettings> options) : base(options)
        {
        }
    }
}
