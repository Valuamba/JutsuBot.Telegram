using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public LinkedStateMachine(ServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            _count = 0;
            _firstNode = null;
            _lastNode = null;
        }

        //TODO: добавить возможность брать стед из строки соответствующей сообщению.
        public ILinkedStateMachine<TContext> Stage(string stage, Action<ILinkedStateMachine<TContext>> branch)
        {
            var stageBranch = new LinkedStateMachine<TContext>(ServiceCollection);
            branch(stageBranch);

            LinkedNode<TContext> newNode = new();

            newNode.Data = async (context, cancellationToken) =>
            {
                if (context.UserState.CurrentState.Stage == stage)
                {
                    await stageBranch.Head.Data(context, cancellationToken);
                }
                else
                {
                    if (newNode.Next is not null)
                    {
                        await newNode.Next.Data(context, cancellationToken);
                    }
                }
            };

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
                            if (node.Step != null)
                            {
                                await node.Step(context);
                            }
                        }
                        else
                        {
                            if ((node.CallbackButtonHandler == null         || !await node.CallbackButtonHandler(context))
                                && (node.ReplyKeyboardButtonHandler == null || !await node.ReplyKeyboardButtonHandler(context)))
                            {
                                if (node.Handler != null)
                                {
                                    await node.Handler(context);
                                }
                            }
                        }
                    };

            return executionSequence ?? defaultExecutionSequence;
        }

        public void SeparateObjectToHandlers<THandler>(
            LinkedNode<TContext> node, 
            THandler handler,
            UpdateDelegate<TContext> prevDelegate = null,
            UpdateDelegate<TContext> nextDelegate = null)
        {
            if (handler is Interfaces.ICallbackButtonHandler<TContext> callbackButtonHandler)
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                    callbackButtonHandler.HandleCallbackButton(context, prevDelegate, nextDelegate, cancellationToken);
            }

            if (handler is IReplyKeyboardButtonHandler<TContext> replyKeyboardButtonHandler)
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                    replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, prevDelegate, nextDelegate, cancellationToken);
            }

            if (handler is IStep<TContext> step)
            {
                node.Step = (context, cancellationToken) =>
                    step.SendStepInformationAsync(context, cancellationToken);
            }

            if (handler is ICallbackButtonHandler<TContext> updateHandler)
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
            var handlerInterfaces = typeof(THandler).GetInterfaces();

            if (handlerInterfaces.Any(i => i == typeof(Interfaces.ICallbackButtonHandler<TContext>)))
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is Interfaces.ICallbackButtonHandler<TContext> callbackButtonHandler)
                        return callbackButtonHandler.HandleCallbackButton(context, prevDelegate, nextDelegate, cancellationToken);
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };  
            }

            if (handlerInterfaces.Any(i => i == typeof(IReplyKeyboardButtonHandler<TContext>)))
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is IReplyKeyboardButtonHandler<TContext> replyKeyboardButtonHandler)
                        return replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, prevDelegate, nextDelegate, cancellationToken);
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(IStep<TContext>)))
            {
                node.Step = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is IStep<TContext> step)
                        return step.SendStepInformationAsync(context, cancellationToken);
                    else
                        throw new PipelineException($"Unable to resolve handler of type {typeof(THandler).FullName}");
                };
            }

            if (handlerInterfaces.Any(i => i == typeof(ICallbackButtonHandler<TContext>)))
            {
                node.Handler = (context, cancellationToken) =>
                {
                    if (context.Services.GetService(typeof(THandler)) is ICallbackButtonHandler<TContext> updateHandler)
                        return updateHandler.HandleAsync(context, prevDelegate, nextDelegate, cancellationToken);
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
            if (callbackButtonHandler is not null)
            {
                node.CallbackButtonHandler =
                    (context, cancellationToken) =>
                        callbackButtonHandler(prevDelegate, nextDelegate, context, cancellationToken);
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
                node.Step =
                    (context, cancellationToken) =>
                        stepHandler(prevDelegate, nextDelegate, context, cancellationToken);
            }
        }

        public void AppendNode(LinkedNode<TContext> node)
        {
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
            ServiceCollection.TryAddScoped(typeof(THandler));
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
