using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuBotServer.Hubs
{
    public class NotificationHub : Hub
    {
        public Task SendMessage(string message)
        {
            return Clients.Caller.SendAsync("Send", message);
        }

        public async Task GetUpdate(object update)
        {
            var name = Context.User.Identity.Name;
            var id = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;
            //var UserAgent = Context.Request.Headers["User-Agent"];
            await Clients.Caller.SendAsync("Send", "Update was received");
            //await Clients.User
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        //public Task SendMessageToUser()
        //{
        //    Clients.User
        //}
    }
}
