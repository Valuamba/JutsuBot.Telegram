using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jutsu.Telegarm.Bot.Models;
using Jutsu.Telegarm.Bot.Models.Interfaces;
using JutsuForms.Server.TgBotFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotFramework
{
    public interface IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContext HttpContext { get; set; }
        public IStageContext StageContext { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
        public IUserState UserState { get; set; }
        public BaseBot Bot { get; set; }
        public IUpdateService BotClient { get; set; }

        Task LeaveStage(string to, CancellationToken cancellationToken, int? step = null);
    }
}