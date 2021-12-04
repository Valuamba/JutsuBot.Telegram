using CliverBot.Console.DataAccess;
using CliverBot.Console.Elements.InputTextElement;
using CliverBot.Console.Extensions;
using CliverBot.Console.Form;
using CliverBot.Console.Form.v3.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Elements.FormInput
{
    public class SmartInputTextElement<TModel, TItem, TContext> : BaseElement<TContext>, IUpdateHandler<TContext>, IStep<TContext>, IReplyKeyboardButtonHandler<TContext> /*: BaseElement<TContext>*/
        where TContext : IUpdateContext
        where TModel : new()
    {
        public List<ValidationHandler<TContext>> ValidationHandlers { get; set; }
        public ReplyKeyboardMarkup EntryReplyKeyboardMarkup { get; set; }
        public ReplyUpdateDelegate<TContext> ReplyUpdateDelegate { get; set; }

        public InputTextLocalizationSettings LocalizationProperty { get; set; }

        public virtual async Task SendValidationError(TContext context, string errorMessage)
        {

        }

        public virtual async Task StorePropertyInCache(TContext context)
        {
            context.UserState.CurrentState.CacheData =
                    context.UserState.CurrentState.CacheData.AddProperty<TModel>(context.Update.Message.Text, PropertyName);
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Type == UpdateType.Message)
            {
                if (ValidationHandlers != null)
                {
                    foreach (var validationHandler in ValidationHandlers)
                    {
                        if (!validationHandler.UpdatePredicate(context))
                        {
                            await SendValidationError(context, validationHandler.ErrorMessage);
                            return;
                        }
                    }
                }

                context.UserState.CurrentState.Step++;
                await StorePropertyInCache(context);

                await next(context);
            }
        }

        public async Task NotifyStep(TContext context, CancellationToken cancellationToken)
        {
            context.UserState.CurrentState.Step++;
            await context.InputTextClient.SendInformationMessage(context.Update.GetSenderId(), NotifyMessage, replyMarkup: EntryReplyKeyboardMarkup);
        }

        public async Task<bool> HandleReplyKeyboardButton(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            return await ReplyUpdateDelegate(prev, next, context, cancellationToken);
        }

        public static StepDelegate<TContext> GetConfirmStepDelegate(string confirmationInfo, List<ResponsibleConfirmation> responsibleConfirmations)
        {
            return async (prev, next, context, cancellationToken) =>
            {
                var memoryRepository = (MemoryRepository)context.Services.GetService(typeof(MemoryRepository));

                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), confirmationInfo);
                context.UserState.CurrentState.Step++;

                foreach (var responsible in responsibleConfirmations)
                {
                    var users = memoryRepository.GetUsersByRole(responsible.Role.Value);
                    foreach (var user in users)
                    {
                        if (responsible.InlineKeyboardMarkup != null)
                        {
                            user.PrevState = user.CurrentState;
                            user.CurrentState = new() { Stage = "confirmAuthorization" };
                        }
                        await context.Client.SendTextMessageAsync(user.Id, responsible.Message, replyMarkup: responsible.InlineKeyboardMarkup);
                    }
                }
            };
        }
    }
}
