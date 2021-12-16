using ConsoleApp1.TgBotFramework.UpdatePipeline;
using JutsuForms.Server.TgBotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework.Exceptions;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework.UpdatePipeline
{
    public class LinkedStateMachine<TContext> : ILinkedStateMachine<TContext> where TContext : IUpdateContext
    {
        private int _count;
        public LinkedNode<TContext> _firstNode { get; private set; }
        private LinkedNode<TContext> _lastNode { get; set; }

        public virtual LinkedNode<TContext> Head
        {
            get { return this._firstNode; }
        }

        public virtual int Count
        {
            get { return this._count; }
        }

        public ServiceCollection ServiceCollection { get; }

        public IServiceProvider ServiceProvider { get; }

        public LinkedStateMachine(IServiceProvider serviceProvider, ServiceCollection serviceCollection)
        {
            ServiceProvider = serviceProvider;
            ServiceCollection = serviceCollection;
            _count = 0;
            _firstNode = null;
            _lastNode = null;
        }

        //TODO: добавить возможность брать стед из строки соответствующей сообщению.
        public ILinkedStateMachine<TContext> Stage(string stage, Action<ILinkedStateMachine<TContext>> branch)
        {
            var stageBranch = new LinkedStateMachine<TContext>(ServiceProvider, ServiceCollection);
            branch(stageBranch);

            LinkedNode<TContext> newNode = new();
            
            newNode.Data = (context, cancellationToken) =>
                context.UserState.CurrentState.Stage.IsStage(stage)
                ? stageBranch.Head.Data(context, cancellationToken)
                : newNode.Next.Data(context, cancellationToken);
           
            AppendNode(newNode);

            return this;
        }

        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecutionSequence(
            Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
        {
            Func<LinkedNode<TContext>, UpdateDelegate<TContext>> defaultExecutionSequence =
                (node) =>
                    async (context, cancellationToken) =>
                    {
                        if (context.UserState?.CurrentState.Step == 0)
                        {
                            await node.Notify(context);
                        }
                        else if (await TryHandleUpdateWithUtilityHandlers(context, node.CallbackButtonHandler, node.ReplyKeyboardButtonHandler))
                        {
                            await node.Handler(context);
                        }
                    };

            return executionSequence ?? defaultExecutionSequence;
        }

        private async Task<bool> TryHandleUpdateWithUtilityHandlers(TContext context, UpdateButtonDelegate<TContext> updateButtonDelegate, UpdateButtonDelegate<TContext> replyKeyboardHandler)
        {
            bool isUtilityHandlerExists;
            try
            {
                isUtilityHandlerExists = !await updateButtonDelegate(context) || !await replyKeyboardHandler(context);
            }
            catch (HandlerNotFoundException ex) when (ex.HandlerCode == (int)HandlerType.CallbackHandler || ex.HandlerCode == (int)HandlerType.ReplyKeyboardHandler)
            {
                isUtilityHandlerExists = true;
            }

            return isUtilityHandlerExists;
        }

        private void InitializeHandlersWithDefaultValue(LinkedNode<TContext> node)
        {
            node.CallbackButtonHandler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.CallbackHandler);
            node.ReplyKeyboardButtonHandler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.ReplyKeyboardHandler);
            node.Notify = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.StepHandler);
            node.Handler = (context, cancellationToken) => throw new HandlerNotFoundException(HandlerType.UpdateHandler);
        }

        public void SeparateObjectToHandlers<THandler>(
            LinkedNode<TContext> node, 
            THandler handler,
            UpdateDelegate<TContext> prevDelegate = null,
            UpdateDelegate<TContext> nextDelegate = null)
        {
            InitializeHandlersWithDefaultValue(node);

            if (handler is ICallbackButtonHandler<TContext> callbackButtonHandler)
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                    callbackButtonHandler.HandleCallbackButton(context, prevDelegate, nextDelegate, cancellationToken);
            }

            if (handler is IReplyKeyboardButtonHandler<TContext> replyKeyboardButtonHandler)
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                    replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, prevDelegate, nextDelegate, cancellationToken);
            }

            if (handler is INotify<TContext> step)
            {
                node.Notify = (context, cancellationToken) =>
                    step.NotifyStep(context, cancellationToken);
            }

            if (handler is IUpdateHandler<TContext> updateHandler)
            {
                node.Handler = (context, cancellationToken) =>
                    updateHandler.HandleAsync(context, prevDelegate, nextDelegate, cancellationToken);
            }
        }

        public void SeparateObjectToHandlers<THandler>(
           LinkedNode<TContext> node,
           UpdateDelegate<TContext> prevDelegate = null,
           UpdateDelegate<TContext> nextDelegate = null)
        {
            InitializeHandlersWithDefaultValue(node);

            var handlerInterfaces = typeof(THandler).GetInterfaces();

            if (handlerInterfaces.Any(i => i == typeof(ICallbackButtonHandler<TContext>)))
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is ICallbackButtonHandler<TContext> callbackButtonHandler)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of callback button with [{typeof(THandler).Name}].");
                        return callbackButtonHandler.HandleCallbackButton(context, prevDelegate, nextDelegate, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };  
            }

            if (handlerInterfaces.Any(i => i == typeof(IReplyKeyboardButtonHandler<TContext>)))
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is IReplyKeyboardButtonHandler<TContext> replyKeyboardButtonHandler)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of reply keyboard with [{typeof(THandler).Name}].");
                        return replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, prevDelegate, nextDelegate, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(INotify<TContext>)))
            {
                node.Notify = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is INotify<TContext> step)
                    {
                        var logger = context.Services.GetRequiredService<ILogger<THandler>>();
                        logger.LogInformation($"Handling of notify with [{typeof(THandler).Name}].");
                        return step.NotifyStep(context, cancellationToken);
                    }
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(IUpdateHandler<TContext>)))
            {
                node.Handler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is IUpdateHandler<TContext> updateHandler)
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

        public void SeparateObjectToHandlers(
            LinkedNode<TContext> node,
            CallbackUpdateDelegate<TContext> callbackButtonHandler,
            ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
            HandlerDelegate<TContext> updateHandler,
            StepDelegate<TContext> stepHandler,
            UpdateDelegate<TContext> prevDelegate = null,
            UpdateDelegate<TContext> nextDelegate = null)
        {
            InitializeHandlersWithDefaultValue(node);

            if (callbackButtonHandler is not null)
            {
                node.CallbackButtonHandler =
                    (context, cancellationToken) =>
                    {
                        var logger = context.Services.GetRequiredService<ILogger>();
                        logger.LogInformation($"The delegate method with name {callbackButtonHandler.Method.Name} is handled.");
                        return callbackButtonHandler(prevDelegate, nextDelegate, context, cancellationToken);
                    };
            }

            if (replyKeyboardButtonHandler is not null)
            {
                node.ReplyKeyboardButtonHandler =
                    (context, cancellationToken) =>
                        replyKeyboardButtonHandler(prevDelegate, nextDelegate, context, cancellationToken);
            }

            if (updateHandler is not null)
            {
                node.Handler =
                    (context, cancellationToken) =>
                        updateHandler(prevDelegate, nextDelegate, context, cancellationToken);
            }

            if (stepHandler is not null)
            {
                node.Notify =
                    (context, cancellationToken) =>
                        stepHandler(prevDelegate, nextDelegate, context, cancellationToken);
            }
        }

        public void AppendNode(LinkedNode<TContext> node)
        {
            UpdateDelegate<TContext> defaultUpdateHandler = async (context, cancellationToken) =>
                    throw new StepNotFoundException(context.UserState.CurrentState.Stage, context.UserState.CurrentState.Step.Value);

            LinkedNode<TContext> defaultLinkedNode = new LinkedNode<TContext>(defaultUpdateHandler);

            node.Previous = defaultLinkedNode;
            node.Next = defaultLinkedNode;

            if (_firstNode == null)
            {
                _firstNode = _lastNode = node;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = node;
                node.Previous = currentNode;
                
                _lastNode = node;
            }

            _count++;
        }

        public ILinkedStateMachine<TContext> Step<THandler>(
               THandler handler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedPrevDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
        {
            //ServiceCollection.TryAddScoped(typeof(THandler));
            LinkedNode<TContext> newNode = new();

            UpdateDelegate<TContext> prevDelegate = extendedPrevDelegate != null
                ? extendedPrevDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Previous?.Data(context, cancellationToken);

            UpdateDelegate<TContext> nextDelegate = extendedNextDelegate != null
                ? extendedNextDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Next?.Data(context, cancellationToken);

            SeparateObjectToHandlers(newNode, handler, prevDelegate, nextDelegate);
            newNode.Data = GetExecutionSequence(executionSequence)(newNode);
            AppendNode(newNode);

            return this;
        }

        public ILinkedStateMachine<TContext> Step<THandler>(
              Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedPrevDelegate = null,
              Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null,
              Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
        {
            ServiceCollection.TryAddScoped(typeof(THandler));
            LinkedNode<TContext> newNode = new();

            UpdateDelegate<TContext> prevDelegate = extendedPrevDelegate != null
                ? extendedPrevDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Previous?.Data(context, cancellationToken);

            UpdateDelegate<TContext> nextDelegate = extendedNextDelegate != null
                ? extendedNextDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Next?.Data(context, cancellationToken);

            SeparateObjectToHandlers<THandler>(newNode, prevDelegate, nextDelegate);
            newNode.Data = GetExecutionSequence(executionSequence)(newNode);
            AppendNode(newNode);

            return this;
        }

        public ILinkedStateMachine<TContext> Step(
               CallbackUpdateDelegate<TContext> callbackButtonHandler,
               ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
               HandlerDelegate<TContext> updateHandler,
               StepDelegate<TContext> stepHandler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedPrevDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
        {
            LinkedNode<TContext> newNode = new();
            
            UpdateDelegate<TContext> prevDelegate = extendedPrevDelegate != null
                ? extendedPrevDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Previous?.Data(context, cancellationToken);

            UpdateDelegate<TContext> nextDelegate = extendedNextDelegate != null
                ? extendedNextDelegate(newNode)
                : async (context, cancellationToken) => await newNode.Next?.Data(context, cancellationToken);

            SeparateObjectToHandlers(newNode, callbackButtonHandler, replyKeyboardButtonHandler, updateHandler, stepHandler,prevDelegate, nextDelegate);
            newNode.Data = GetExecutionSequence(executionSequence)(newNode);
            AppendNode(newNode);

            return this;
        }
    }
}
