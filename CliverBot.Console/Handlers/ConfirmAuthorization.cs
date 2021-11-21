using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;

namespace CliverBot.Console.Handlers
{
    public class ConfirmAuthorization : ICallbackButtonHandler<BotExampleContext>
    {
        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if(context.Update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                return false;
            }

            return true;
        }
    }
}
