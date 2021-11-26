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
    public class MatchStep : TgBotFramework.Interfaces.ICallbackButtonHandler<BotExampleContext>, IStep<BotExampleContext>
    {
        private readonly MatchRepository _matchRepository;

        public MatchStep(MatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if(context.Update.IsCallbackCommand("notify"))
            {
                context.UserState.CurrentState.Step++;
                await next(context, cancellationToken);
                return true;
            }
            else if(context.Update.IsCallbackCommand("back"))
            {
                context.UserState.CurrentState.Step -= 3;
                context.Update.CallbackQuery.Data = null;
                await prev(context, cancellationToken);
            }

            return false;
        }

        public async Task SendStepInformationAsync(BotExampleContext context, CancellationToken cancellationToken)
        {
            if(context.StageContext.Parameters.IsMatchPatter("matchId/"))
            {
                var matchId = Convert.ToInt32(context.StageContext.Parameters.TrimStringPattern("matchId/"));

                var match = await _matchRepository.GetMatchById(matchId);

                var matchMessage = MapMatchToString(match);

                if (On.CallbackQuery(context))
                {
                    context.UserState.CurrentState.Step++;
                    await context.Client.EditMessageTextAsync(context.Update.GetSenderId(), context.Update.CallbackQuery.Message.MessageId, matchMessage,
                        replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
                        {
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("Напомнить") { CallbackData = $"notify/matchId/{matchId}"} },
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("Назад") { CallbackData = "back"} },
                        }));
                } 
                else if(On.Message(context))
                {
                    context.UserState.CurrentState.Step++;
                    await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), matchMessage, 
                        replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
                        {
                            new List<InlineKeyboardButton>() { new InlineKeyboardButton("Напомнить") { CallbackData = $"notify/matchId/{matchId}" } },
                        }));
                }
            }
        }

        public static string MapMatchToString(CSGOMatch match)
        {
            string matchMessage =
                $"🕑 Дата: {match.MatchTime.ToString("dd dddd yyyy HH:mm")}\r\n" +
                $"🔄 Формат игры: {match.MatchMeta}\r\n" +
                $"🆚 Команды: {match.FirstTeam} vs {match.SecondTeam}\r\n" +
                $"📛 Турнир: {match.MatchEvent}\r\n";

            return matchMessage;
        }
    }
}
