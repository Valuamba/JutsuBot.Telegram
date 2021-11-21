using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotFramework
{
    public interface IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContext HttpContext { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
        public IUserState UserState { get; set; }
        public BaseBot Bot { get; set; }
        public TelegramBotClient Client { get; set; }
    }
}