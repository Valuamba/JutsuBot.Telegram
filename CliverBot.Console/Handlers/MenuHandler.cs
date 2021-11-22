using CliverBot.Console.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Handlers
{
    public class MenuHandler : IStep<BotExampleContext>, ICallbackButtonHandler<BotExampleContext>
    {
        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.IsCallbackCommand("addPartner"))
            {
                var chanel = (Channel<IUpdateContext>)context.Services.GetService(typeof(Channel<IUpdateContext>));

                context.UserState.PrevState = context.UserState.CurrentState;
                context.UserState.CurrentState.Step = 0;
                context.UserState.CurrentState.Stage = "addPartner";

                var userId = context.Update.GetSenderId();

                var update = UpdateExtensions.EchoMessageUpdate(userId, userId);

                BotExampleContext newContext = new()
                {
                    Services = context.Services,
                    Client = context.Client,
                    Bot = context.Bot,
                    Update = update
                };

                await chanel.Writer.WriteAsync(newContext, cancellationToken);

                return true;
            }

            return false;
        }

        public async Task SendStepInformationAsync(BotExampleContext context, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Вы прошли авторизацию успешно, наслаждайтесь главным меню!", 
                replyMarkup: (InlineKeyboardMarkup) new IEnumerable<InlineKeyboardButton>[]
                {
                    new InlineKeyboardButton[] { new("Добавить партнера") {CallbackData = "addPartner"}}
                });
            context.UserState.CurrentState.Step++;
        }
    }
}
