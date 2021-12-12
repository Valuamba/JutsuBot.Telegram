using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBotFramework;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using JutsuBot.Elements.DataAccess;
using Microsoft.EntityFrameworkCore;
using EntityUser = CliverBot.Console.DataAccess.User;
using JutsuForms.Server.FormBot.DataAccess.Entities;

namespace JutsuForms.Server.TgBotFramework.Hubs
{
    public class UpdateHub : Hub
    {
        private readonly ChannelWriter<IUpdateContext> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;

        public UpdateHub(Channel<IUpdateContext> channel, IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            _channel = channel.Writer;
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public async Task GetUpdate(Update update)
        {
            var updateContext = _serviceProvider.GetService<IUpdateContext>();

            updateContext.Update = update;
            //updateContext.BotClient = new UpdateService(Clients, _serviceProvider);

            await _channel.WriteAsync(updateContext, CancellationToken.None);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Convert.ToInt64(Context.GetHttpContext().Request.Headers["userId"].ToString());
            var user = _dbContext.Users
                    .Include(u => u.Connections)
                    .SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                user = new EntityUser
                {
                    Id = userId,
                    Connections = new List<Connection>()
                };
                _dbContext.Users.Add(user);
            }

            user.Connections.Add(new Connection
            {
                ConnectionId = Context.ConnectionId,
                UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"],
                Connected = true
            });

            await _dbContext.SaveChangesAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = _dbContext.Connections.Find(Context.ConnectionId);
            connection.Connected = false;
            _dbContext.SaveChanges();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
