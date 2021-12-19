using ConsoleApp1;
using ConsoleApp1.TgBotFramework.UpdatePipeline;
using JutsuForms.Server.FormBot.Handlers;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.TgBotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Exceptions;
using TgBotFramework.Interfaces;
using TgBotFramework.UpdatePipeline;

namespace JutsuForms.Server.FormBot
{
    public static class FormLinkedStateMachineExtensions
    {
        public static ILinkedStateMachine<BotExampleContext> Form(this ILinkedStateMachine<BotExampleContext> pipe, string stage, Action<ILinkedStateMachine<BotExampleContext>> branch)
        {
            var stageBranch = new LinkedStateMachine<BotExampleContext>(pipe.ServiceProvider, pipe.ServiceCollection);
            branch(stageBranch);

            LinkedNode<BotExampleContext> newNode = new();

            newNode.Data = (context, cancellationToken) =>
                context.UserState.CurrentState.Stage.IsStage(stage)
                ? stageBranch.Head.Data(context, cancellationToken)
                : newNode.Next.Data(context, cancellationToken);

            pipe.AppendNode(newNode);

            return pipe;
        }

        public static Func<LinkedNode<BotExampleContext>, UpdateDelegate<BotExampleContext>> GetStepExecutionSequence(int notifyStep)
                => (node) => async (context, cancellationToken) =>
                {
                    int handlerStep = notifyStep + 1;
                    if (context.UserState?.CurrentState.Step == notifyStep)
                    {
                        await node.Notify(context);
                    }
                    else if (context.UserState?.CurrentState.Step == handlerStep)
                    {
                        if (await TryHandleUpdateWithUtilityHandlers(context, node.CallbackButtonHandler, node.ReplyKeyboardButtonHandler))
                        {
                            await node.Handler(context);
                        }
                    }
                    else
                    {
                        await node.Next.Data(context, cancellationToken);
                    }
                };

        private static async Task<bool> TryHandleUpdateWithUtilityHandlers(BotExampleContext context, UpdateButtonDelegate<BotExampleContext> updateButtonDelegate, UpdateButtonDelegate<BotExampleContext> replyKeyboardHandler)
        {
            bool isUtilityHandlerExists = true;
            try
            {
                if (await updateButtonDelegate(context))
                    return false;
            }
            catch (HandlerNotFoundException ex) when (ex.HandlerCode == (int)HandlerType.CallbackHandler || ex.HandlerCode == (int)HandlerType.ReplyKeyboardHandler)
            {
            }

            try
            {
                if (await replyKeyboardHandler(context))
                    return false;
            }
            catch (HandlerNotFoundException ex) when (ex.HandlerCode == (int)HandlerType.CallbackHandler || ex.HandlerCode == (int)HandlerType.ReplyKeyboardHandler)
            {
            }

            return isUtilityHandlerExists;
        }

        private static THandler InitializeFormHandler<THandler>(
            IServiceProvider serviceProvider,
            FormHandlerContext formHandlerContext,
            FormContext formContext)
                where THandler : AuthorizationBaseHandler
        {
            var constructorInfo = typeof(THandler).GetConstructors().SingleOrDefault();
            List<object> arguments = new List<object>();

            foreach (var parameter in constructorInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(FormHandlerContext))
                {
                    arguments.Add(formHandlerContext);
                }
                else if (parameter.ParameterType == typeof(FormContext))
                {
                    arguments.Add(formContext);
                }
                else
                {
                    arguments.Add(serviceProvider.GetService(parameter.ParameterType));
                }
            }
            return (THandler) Activator.CreateInstance(typeof(THandler), args: arguments.ToArray());
        }

        public static ILinkedStateMachine<BotExampleContext> FormStep<THandler>(
                this ILinkedStateMachine<BotExampleContext> pipe,
                FormHandlerContext formHandlerContext)
            where THandler : AuthorizationBaseHandler
        {
            var formContext = pipe.ServiceProvider.GetRequiredService<FormContext>();
            formContext.AddFormHandlerContext(formHandlerContext);

            //ServiceCollection.TryAddScoped(typeof(THandler));
            LinkedNode<BotExampleContext> newNode = new();
            UpdateDelegate<BotExampleContext> prevDelegate = async (context, cancellationToken) => await newNode.Previous?.Data(context, cancellationToken);
            UpdateDelegate<BotExampleContext> nextDelegate = async (context, cancellationToken) => await newNode.Next?.Data(context, cancellationToken);

            SeparateObjectToHandlers<THandler>(newNode, formHandlerContext, formContext, prevDelegate, nextDelegate);
            newNode.Data = GetStepExecutionSequence(formHandlerContext.Step)(newNode);
            pipe.AppendNode(newNode);

            return pipe;
        }

        public static void SeparateObjectToHandlers<THandler>(
          LinkedNode<BotExampleContext> node,
          FormHandlerContext formHandlerContext,
          FormContext formContext,
          UpdateDelegate<BotExampleContext> prevDelegate = null,
          UpdateDelegate<BotExampleContext> nextDelegate = null)
            where THandler : AuthorizationBaseHandler
        {
            InitializeHandlersWithDefaultValue(node);

            var handlerInterfaces = typeof(THandler).GetInterfaces();

            if (handlerInterfaces.Any(i => i == typeof(ICallbackButtonHandler<BotExampleContext>)))
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                {
                    var handler = InitializeFormHandler<THandler>(context.Services, formHandlerContext, formContext);
                    if (handler is ICallbackButtonHandler<BotExampleContext> callbackButtonHandler)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of callback button with [{typeof(THandler).Name}].");
                        return callbackButtonHandler.HandleCallbackButton(context, prevDelegate, nextDelegate, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(IReplyKeyboardButtonHandler<BotExampleContext>)))
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                {
                    var handler = InitializeFormHandler<THandler>(context.Services, formHandlerContext, formContext);
                    if (handler is IReplyKeyboardButtonHandler<BotExampleContext> replyKeyboardButtonHandler)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of reply keyboard with [{typeof(THandler).Name}].");
                        return replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, prevDelegate, nextDelegate, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(INotify<BotExampleContext>)))
            {
                node.Notify = (context, cancellationToken) =>
                {
                    var handler = InitializeFormHandler<THandler>(context.Services, formHandlerContext, formContext);
                    if (handler is INotify<BotExampleContext> step)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of notify with [{typeof(THandler).Name}].");
                        return step.NotifyStep(context, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(IUpdateHandler<BotExampleContext>)))
            {
                node.Handler = (context, cancellationToken) =>
                {
                    var handler = InitializeFormHandler<THandler>(context.Services, formHandlerContext, formContext);
                    if (handler is IUpdateHandler<BotExampleContext> updateHandler)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of update with [{typeof(THandler).Name}].");
                        return updateHandler.HandleAsync(context, prevDelegate, nextDelegate, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }
        }

        private static void InitializeHandlersWithDefaultValue(LinkedNode<BotExampleContext> node)
        {
            node.CallbackButtonHandler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.CallbackHandler);
            node.ReplyKeyboardButtonHandler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.ReplyKeyboardHandler);
            node.Notify = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.StepHandler);
            node.Handler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.UpdateHandler);
        }
    }
}
