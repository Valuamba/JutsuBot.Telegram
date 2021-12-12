using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuBot.Elements.DataAccess;
using JutsuForms.Server.FormBot.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationConfirmationRequest : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationConfirmationRequest(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            await context.BotClient.SendTextMessageAsync(context.Update.GetSenderId(), "Wait for acceptance.");
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var admins = _context.Users.Include(u => u.CurrentState).Where(u => u.Role == Role.Admin).ToList();

            if(admins.Count == 0)
                throw new AuthorizationBotException("There is no users that can handle authorization confimation request.");

            foreach (var admin in admins)
            {
                admin.CurrentState = new CliverBot.Console.DataAccess.State()
                {
                    Step = 0,
                    Stage = "authorization",
                    StatePriority = Jutsu.Telegarm.Bot.Models.StatePriority.Medium,
                    UserId = admin.Id
                };
                await context.BotClient.SendTextMessageAsync(admin.Id, "User wants to sign in.\r\nPleace confirm:\r\n1 — Accept\r\n2 — Desline");
            }

            await context.BotClient.SendTextMessageAsync(context.Update.GetSenderId(), "Pleace, waiting for confirmation.");
            context.UserState.CurrentState.Step++;
        }
    }
}
