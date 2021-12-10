using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationInterestsHandler : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            //Console.WriteLine
        }

        public Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
