using ConsoleApp1;
using JutsuBot.Elements.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;

namespace JutsuForms.Server.FormBot.Handlers
{
    public class ResolveAuthorizationHandler : IUpdateHandler<BotExampleContext>
    {
        private readonly ApplicationDbContext _context;

        public ResolveAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (int.TryParse(context.Update.Message.Text, out int value))
            {
                switch (value)
                {
                    case 1:
                        var user = _context.Users.Include(u => u.CurrentState).Single(u => u.Id == 1111);
                        user.CurrentState.Step++;
                        await _context.SaveChangesAsync();
                        await context.SendUpdate(user.Id, cancellationToken);
                        break;
                }
            }
        }
    }
}
