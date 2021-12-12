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
                //Нужно получать ID из колбэка
                var user = _context.Users.Include(u => u.CurrentState).Single(u => u.Id == 1111);

                switch (value)
                {
                    case 1:
                        user.CurrentState.Step++;
                        user.CurrentState.Stage = $"authorization?result={true}";
                        break;

                    case 2:
                        user.CurrentState.Step++;
                        user.CurrentState.Stage = $"authorization?result={false}";
                        break;
                }

                await _context.SaveChangesAsync();
                await context.SendUpdate(user.Id, cancellationToken);
            }
        }
    }
}
