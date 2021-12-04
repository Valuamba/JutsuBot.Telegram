﻿using CliverBot.Console.DataAccess;
using CliverBot.Console.Form;
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

        public static HandlerDelegate<TContext> GetHandlerDelegate<TContext>(UpdateType expectedUpdateType, List<ValidationHandler<TContext>> validationHandlers)
             where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                if (context.Update.Type == expectedUpdateType)
                {
                    if (validationHandlers != null)
                    {
                        foreach (var validationHandler in validationHandlers)
                        {
                            if (!validationHandler.UpdatePredicate(context))
                            {
                                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), validationHandler.ErrorMessageAlias);
                                return;
                            }
                        }
                    }

                    context.UserState.CurrentState.CacheData += context.Update.Message.Text;

                    await next(context);
                }
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

        private static StepDelegate<TContext> GetConfirmStepDelegate<TContext>(string confirmationInfo, List<ResponsibleConfirmation> responsibleConfirmations)
            where TContext : IUpdateContext
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

        //public static ILinkedStateMachine<TContext> AddForm<T, TContext>(this ILinkedStateMachine<TContext> pipeline, Action<IFormHandlerBuilder<TContext>> formBuilderConfigurator)
        //    where TContext : IUpdateContext
        //{
        //    FormHandlerBuilder<TContext> formHandler = new();
        //    formBuilderConfigurator(formHandler);

        //    foreach (var form in formHandler.FormFields)
        //    {
        //        var formBuilder = new FormBuilder<T, TContext>(form, formHandler.Stage);

        //        pipeline.Step(
        //            callbackButtonHandler: form.CallbackKeyboardHandler,
        //            replyKeyboardButtonHandler: form.ReplyKeyboardHandler,
        //            updateHandler: formBuilder.UpdateHandler,
        //            stepHandler: formBuilder.StepDelegate,
        //            extendedPrevDelegate: formHandler.ExtendedPrevDelegate,
        //            extendedNextDelegate: formHandler.ExtendedNextDelegate,
        //            executionSequence: formBuilder.ExecuteSequence);
        //    }

        //    var confirmationInfo = formHandler.ConfiramtionInfo;
        //    var confirmAuthorizationDelegate = GetConfirmStepDelegate<TContext>(confirmationInfo.ConfirmationText, confirmationInfo.ResponsiblesToConfirmation);

        //    pipeline.Step(
        //            callbackButtonHandler: null,
        //            replyKeyboardButtonHandler: null,
        //            updateHandler: null,
        //            stepHandler: confirmAuthorizationDelegate,
        //            executionSequence: (node) => async (context, cancellationToken) =>
        //            {
        //                if(context.UserState.CurrentState.Step == confirmationInfo.Step)
        //                {
        //                    await node.Step(context);
        //                }
        //            });

        //    return pipeline;
        //}
    }
}
