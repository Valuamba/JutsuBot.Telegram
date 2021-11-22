using CliverBot.Console.DataAccess;
using CliverBot.Console.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console
{
    public static class BotPipelineExtensions
    {
        public static StepDelegate<TContext> GetNotifyMethod<TContext>(string notifyText, IReplyMarkup? replyMarkup)
            where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                context.UserState.CurrentState.Step++;
                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), notifyText, replyMarkup: replyMarkup);
            };
        }

        public static HandlerDelegate<TContext> GetHandlerDelegate<TContext>(List<ValidationHandler<TContext>> validationHandlers)
             where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                if (validationHandlers != null)
                {
                    foreach (var validationHandler in validationHandlers)
                    {
                        if (!validationHandler.UpdatePredicate(context))
                        {
                            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), validationHandler.ErrorMessage);
                            return;
                        }
                    }
                }

                context.UserState.CurrentState.CacheData += context.Update.Message.Text;

                await next(context);
            };
        }

        private static Predicate<TContext> NodePredicate<TContext>(int notifyStep, int handlerStep, string formStage)
            where TContext : IUpdateContext
                => (context) => 
                    (context.UserState.CurrentState.Step == notifyStep || context.UserState.CurrentState.Step == handlerStep) 
                    && context.UserState.CurrentState.Stage == formStage;

        public static Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecuteSequence<TContext>(Predicate<TContext> condition, int notifyStep, int handlerStep)
            where TContext : IUpdateContext
        {
            return (node) => async (context, cancellationToken) =>
            {
                if (condition(context))
                {
                    if (context.UserState?.CurrentState.Step == notifyStep)
                    {
                        await node.Step(context);
                    }
                    else if (context.UserState?.CurrentState.Step == handlerStep)
                    {
                        if ((node.CallbackButtonHandler == null || !await node.CallbackButtonHandler(context))
                         && (node.ReplyKeyboardButtonHandler == null || !await node.ReplyKeyboardButtonHandler(context)))
                        {
                            await node.Handler(context);
                        }
                    }
                    else
                    {
                        await node.Next.Data(context);
                    }
                }
                else
                {
                    if (node.Next != null)
                    {
                        await node.Next.Data(context);
                    }
                }
            };
        }

        private static StepDelegate<TContext> GetConfirmStepDelegate<TContext>(string confirmationInfo, MemoryRepository memoryRepository, List<ResponsibleConfirmation> responsibleConfirmations)
            where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
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

        public static ILinkedStateMachine<TContext> AddForm<TContext>(this ILinkedStateMachine<TContext> pipeline, Action<IFormHandlerBuilder<TContext>> formBuilderConfigurator, MemoryRepository memoryRepository)
            where TContext : IUpdateContext
        {
            FormHandlerBuilder<TContext> formHandler = new();
            formBuilderConfigurator(formHandler);
            LinkedStateMachine<TContext> stateMachine = new();

            foreach (var form in formHandler.FormFields)
            {
                LinkedNode<TContext> newNode = new();

                int notifyStep = form.Step;
                int handlerStep = form.Step + 1;

                IReplyMarkup replyMarkup = (IReplyMarkup) form.EntryReplyKeyboardMarkup ?? form.EntryInlineKeyboardMarkup;

                var nodePredicate = NodePredicate<TContext>(notifyStep, handlerStep, formHandler.Stage);
                var updateHandler = GetHandlerDelegate(form.ValidationHandlers);
                var stepHandler = GetNotifyMethod<TContext>(form.InformationText, replyMarkup);
                var executionSequence = GetExecuteSequence(nodePredicate, notifyStep, handlerStep);

                pipeline.Use(
                    callbackButtonHandler: form.CallbackKeyboardHandler,
                    replyKeyboardButtonHandler: form.ReplyKeyboardHandler,
                    updateHandler: updateHandler,
                    stepHandler: stepHandler,
                    extendedPrevDelegate: formHandler.ExtendedPrevDelegate,
                    extendedNextDelegate: formHandler.ExtendedNextDelegate,
                    executionSequence: executionSequence);
            }

            var confirmationInfo = formHandler.ConfiramtionInfo;
            var confirmAuthorizationDelegate = GetConfirmStepDelegate<TContext>(confirmationInfo.ConfirmationText, memoryRepository, confirmationInfo.ResponsiblesToConfirmation);

            pipeline.Use(
                    callbackButtonHandler: null,
                    replyKeyboardButtonHandler: null,
                    updateHandler: null,
                    stepHandler: confirmAuthorizationDelegate,
                    executionSequence: (node) => async (context, cancellationToken) =>
                    {
                        if(context.UserState.CurrentState.Step == confirmationInfo.Step)
                        {
                            await node.Step(context);
                        }
                    });

            return pipeline;
        }
    }
}
