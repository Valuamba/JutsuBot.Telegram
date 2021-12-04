using CliverBot.Console;
using Htlv.Parser.DataAccess.EF.Repositories;
using Htlv.Parser.Extensions;
using Htlv.Parser.Models;
using Jutsu.Telegarm.Bot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace Htlv.Parser.Steps.MatchStage
{
    public class NotifyStep : IStep<BotExampleContext>, TgBotFramework.Interfaces.ICallbackButtonHandler<BotExampleContext>
    {
        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.IsCallbackCommand("back/matchId/"))
            {
                var matchId = Convert.ToInt32(context.Update.TrimCallbackCommand("back/matchId/"));

                context.StageContext.Parameters = $"matchId/{matchId}";

                context.UserState.CurrentState.Step -= 3;
                context.Update.CallbackQuery.Data = null;
                await prev(context, cancellationToken);

                return true;
            }

            return false;
        }

        //private readonly MatchRepository _matchRepository;

        //public NotifyStep(MatchRepository matchRepository)
        //{
        //    _matchRepository = matchRepository;
        //}

        //Запомнить и передать в Назад
        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            if (context.Update.IsCallbackCommand("notify/matchId/"))
            {
                var matchId = Convert.ToInt32(context.Update.TrimCallbackCommand("notify/matchId/"));
                context.UserState.CurrentState.Step++;
                await context.Client.EditMessageTextAsync(context.Update.GetSenderId(), context.Update.CallbackQuery.Message.MessageId,
                    "Выберите за сколько вам напомнить.",
                    replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
                    {
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 15 мин") { CallbackData = $"notify/15minutes/matchId/{matchId}" } },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 30 мин") { CallbackData = $"notify/30minutes/matchId/{matchId}" } },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 1 час") { CallbackData = $"notify/1hour/matchId/{matchId}" } },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 1 день") { CallbackData = $"notify/1days/matchId/{matchId}" } },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("Назад") { CallbackData = $"back/matchId/{matchId}" } },
                    }));
            }
            else if (On.Message(context))
            {
                context.UserState.CurrentState.Step++;
                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(),
                    "Выберите за сколько вам напомнить.",
                    replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
                    {
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 15 мин") { CallbackData = "notify"} },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 30 мин") { CallbackData = "notify"} },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 1 час") { CallbackData = "notify"} },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("За 1 день") { CallbackData = "notify"} },
                    }));
            }
        }
    }
}
