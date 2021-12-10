using Jutsu.Telegarm.Bot.Models;
using JutsuBot.Elements.DataAccess;
using JutsuBot.Elements.DataAccess.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.DataAccess
{
    public class StateMapperMiddleware<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly StateRepository _stateRepository;
        private readonly ApplicationDbContext _context;

        public StateMapperMiddleware(StateRepository stateRepository, ApplicationDbContext context)
        {
            _stateRepository = stateRepository;
            _context = context;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            State state = null;
            var mainState = _stateRepository.GetStateByPriority(StatePriority.Main, context.Update.GetSenderId());
            if (mainState is null)
            {
                if (On.CallbackQuery(context))
                {
                    var messageId = context.Update?.CallbackQuery?.Message?.MessageId;
                    state = _stateRepository.GetStateByMessageId(context.Update.GetSenderId(), messageId.Value);
                }
                else
                {
                    var mediumState = _stateRepository.GetStateByPriority(StatePriority.Medium, context.Update.GetSenderId());
                    if(mediumState is null)
                    {
                        state = new();
                        state.Stage = "menu";
                        state.Step = 0;
                        state.StatePriority = StatePriority.Medium;
                        state.UserId = context.Update.GetSenderId();

                        state = _stateRepository.AddState(state);
                    }
                    else
                    {
                        state = mediumState;
                    }
                }
            }
            else
            {
                state = mainState;
            }

            context.UserState.CurrentState ??= new();
            StateMapper.MapModelToState(context.UserState.CurrentState, state);

            await next(context, cancellationToken);

            if(StateMapper.MapFrameworkStateToEntityState(context.UserState.CurrentState, state))
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
