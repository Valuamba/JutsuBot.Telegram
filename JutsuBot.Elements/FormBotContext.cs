using CliverBot.Console.Elements.InputTextElement;
using Jutsu.Telegarm.Bot.Models;
using Jutsu.Telegarm.Bot.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework;

namespace JutsuBot.Elements
{
    public class FormBotContext : BaseUpdateContext, IInputTextElementContext
    {
        public IInputTextClient<IInputTextElementContext> InputTextClient { get; set; }
    }
}
