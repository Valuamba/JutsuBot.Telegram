using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;
using Telegram.Bot.Types;
using System.Threading.Channels;
using CliverBot.Console.DataAccess;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Handlers
{
    public class ConfirmAuthorization : ICallbackButtonHandler<BotExampleContext>
    {
        private readonly MemoryRepository _memoryRepository;

        public ConfirmAuthorization(MemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if(context.Update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                var chanel = (Channel<IUpdateContext>)context.Services.GetService(typeof(Channel<IUpdateContext>));

                var userId = Convert.ToInt64(context.Update.CallbackQuery.Data);
                var user = _memoryRepository.GetUserById(userId);

                user.CurrentState.Step = 0;
                user.CurrentState.Stage = "menu";

                var update = UpdateExtensions.EchoMessageUpdate(userId, userId);

                BotExampleContext newContext = new()
                {
                    Services = context.Services,
                    Client = context.Client,
                    Bot = context.Bot,
                    Update = update
                };

                await chanel.Writer.WriteAsync(newContext, cancellationToken);

                return false;
            }

            return true;
        }
    }
}
