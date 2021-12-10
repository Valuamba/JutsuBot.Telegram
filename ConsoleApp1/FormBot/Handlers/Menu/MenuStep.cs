using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;

namespace ConsoleApp1.FormBot.Handlers.Menu
{
    public class MenuStep : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if(On.Message(context))
            {
                if(int.TryParse(context.Update.Message.Text, out int value))
                {
                    switch(value)
                    {
                        case 1: await context.LeaveStage("authorization", cancellationToken); break;
                    }
                }
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("You are in main menu.");
            Console.WriteLine("1 - Authorization");
            context.UserState.CurrentState.Step++;
        }
    }
}
