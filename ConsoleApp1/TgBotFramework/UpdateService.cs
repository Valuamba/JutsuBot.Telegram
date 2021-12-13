using CliverBot.Console.DataAccess;
using JutsuBot.Elements.DataAccess;
using JutsuForms.Server.TgBotFramework.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JutsuForms.Server.TgBotFramework
{
    public class UpdateService : IUpdateService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<UpdateHub> _hubContext;

        public UpdateService(IHubContext<UpdateHub> hubContext, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        private async Task<CliverBot.Console.DataAccess.User> GetUserWithConnectionsAsync(long userId)
        {
            return await _dbContext.Users.Include(u => u.Connections).SingleAsync(u => u.Id == userId);
        }

        public async Task<Message> SendTextMessageAsync(Telegram.Bot.Types.ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<Telegram.Bot.Types.MessageEntity> entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            var user = await GetUserWithConnectionsAsync((long)chatId.Identifier);

            foreach (var connection in user.Connections)
            {
                await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("Send", text);
            }
            return null;
        }
    }
}
