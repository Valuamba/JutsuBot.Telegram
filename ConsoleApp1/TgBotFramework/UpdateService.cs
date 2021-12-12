using CliverBot.Console.DataAccess;
using JutsuBot.Elements.DataAccess;
using JutsuForms.Server.TgBotFramework.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.TgBotFramework
{
    public class UpdateService : IUpdateService
    {
        private readonly IHubCallerClients _clients;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<UpdateHub> _hubContext;

        public UpdateService(IHubContext<UpdateHub> hubContext, ApplicationDbContext dbContext)
        {
            //_clients = clients;
            //_serviceProvider = serviceProvider;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        private async Task<User> GetUserWithConnectionsAsync(long userId)
        {
            //var _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            return await _dbContext.Users.Include(u => u.Connections).SingleAsync(u => u.Id == userId);
        }

        public async Task SendMessage(long userId, string message)
        {
            var user = await GetUserWithConnectionsAsync(userId);

            foreach(var connection in user.Connections)
            {
                await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("Send", message);
            }
        }
    }
}
