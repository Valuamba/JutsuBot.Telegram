using Htlv.Parser.DataAccess.EF.Repositories;
using Htlv.Parser.Models;
using Htlv.Parser.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Telegram.Bot.Types.ReplyMarkups;
using Jutsu.Telegarm.Bot.Extensions;
using Telegram.Bot;
using TgBotFramework.WrapperExtensions;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Htlv.Parser.Pagination;

namespace Htlv.Parser.Steps.MatchStage
{
    public static class MatchSelectorBotPipelineExensions
    {
        public const string Match_CallData = "match_id/";

        public static ILinkedStateMachine<TContext> AddMatchSelector<TContext>(this ILinkedStateMachine<TContext> pipeline)
            where TContext : IUpdateContext
        {
            pipeline.AddSelector<CSGOMatch, TContext>(config =>
            {
                config.InformationText = async (context) => "Расписание матчей";

                config.DataConfigurator = async (context) =>
                {
                    var matchRepository = (MatchRepository)context.Services.GetService(typeof(MatchRepository));

                    return matchRepository.GetActualMatchesQuery();
                };

                config.CallbackDataTemplate = (match) => $"match_id/{match.Id}";
                config.InlineButtonTextTemplate = (match) => match.MatchEvent;

                config.HandleSelectedItem = SelectMatchHandler<TContext>();

                config.NavigationButtons = new List<List<InlineKeyboardButton>>()
                {
                    new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Назад", "back/")}
                };

                config.NavigationButtonsHandler = async (prev, next, context, cancellationToken) =>
                {
                    if (context.Update.IsCallbackCommand("back/"))
                    {
                        await context.LeaveStage("menu", cancellationToken);
                        return true;
                    }

                    return false;
                };

                config.ExtendedPrevDelegate = (node) => async (context, cancellationToken) =>
                {
                    context.Update.ClearUpdate();
                    context.UserState.CurrentState.Step -= 3;
                    await node.Previous?.Data(context, cancellationToken);
                };

                config.ExtendedNextDelegate = (node) => async (context, cancellationToken) =>
                {
                    context.Update.ClearUpdate();
                    context.UserState.CurrentState.Step++;
                    await node.Next?.Data(context, cancellationToken);
                };

                config.PaginationConfigurator = (pagOptions) =>
                {
                    pagOptions.Pagination = new NextPrevPagination()
                    {
                        MaxElementsCount = 8,
                    };
                };
            });


            return pipeline;
        }

        public static HandlerDelegate<TContext> SelectMatchHandler<TContext>()
            where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                if (context.Update.IsCallbackCommand(Match_CallData))
                {
                    var matchId = context.Update.TrimCallbackCommand(Match_CallData);

                    context.StageContext.Parameters = $"matchId/{matchId}";

                    await next(context, cancellationToken);
                }
            };
        }
    }
}
