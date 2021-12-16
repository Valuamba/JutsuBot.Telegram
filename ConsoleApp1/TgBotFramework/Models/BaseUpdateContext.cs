using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Jutsu.Telegarm.Bot.Models;
using Jutsu.Telegarm.Bot.Models.Interfaces;
using JutsuForms.Server.TgBotFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework
{
    public class BaseUpdateContext : IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContext HttpContext { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
        public IUserState UserState { get; set; } 
        public BaseBot Bot { get; set; }
        public IStageContext StageContext { get; set; }
        public IUpdateService BotClient { get; set; }

        public async Task MooveToRoot(CancellationToken cancellationToken)
        {
            var channel = (Channel<IUpdateContext>)Services.GetService(typeof(Channel<IUpdateContext>));
            Update.ClearUpdate();

            await channel.Writer.WriteAsync(this, cancellationToken);
        }

        public async Task LeaveStage(string to, CancellationToken cancellationToken, int? step = null)
        {
            UserState.PrevState = UserState.CurrentState;
            UserState.CurrentState.CacheData = null;
            UserState.CurrentState.Stage = to;
            UserState.CurrentState.Step = step ?? 0;

            var channel = (Channel<IUpdateContext>) Services.GetService(typeof(Channel<IUpdateContext>));
            Update.ClearUpdate();

            await channel.Writer.WriteAsync(this, cancellationToken);
        }

        public async Task SendUpdate(long chatId, CancellationToken cancellationToken)
        {
            var channel = (Channel<IUpdateContext>)Services.GetService(typeof(Channel<IUpdateContext>));
            Update.ClearUpdate(chatId);

            await channel.Writer.WriteAsync(this, cancellationToken);
        }
    }
}