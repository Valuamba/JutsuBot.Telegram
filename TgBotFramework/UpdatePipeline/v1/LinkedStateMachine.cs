using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework.Interfaces;

namespace TgBotFramework.UpdatePipeline.v1
{
    public class LinkedStateMachine<TContext> where TContext : IUpdateContext
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

        public LinkedStateMachine()
        {
            _count = 0;
            _firstNode = null;
            _lastNode = null;
        }

        public LinkedStateMachine<TContext> Stage(string stage, Action<LinkedStateMachine<TContext>> branch)
        {
            var stageBranch = new LinkedStateMachine<TContext>();
            branch(stageBranch);

            Use(new UseWhenMiddleware<TContext>((context) => context.UserState.CurrentState.Stage == stage, stageBranch.Head.Data));

            return this;
        }

        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecutionSequence(Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
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
                            if ((node.CallbackButtonHandler == null || !await node.CallbackButtonHandler(context))
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
            THandler handler)
            where THandler : IUpdateHandler<TContext>
        {
            if (handler is ICallbackButtonHandler<TContext> callbackButtonHandler)
            {
                node.CallbackButtonHandler = (context, cancellationToken) =>
                    callbackButtonHandler.HandleCallbackButton(context, node.Previous?.Step, node.Next?.Step, cancellationToken);
            }

            if (handler is IReplyKeyboardButtonHandler<TContext> replyKeyboardButtonHandler)
            {
                node.ReplyKeyboardButtonHandler = (context, cancellationToken) =>
                    replyKeyboardButtonHandler.HandleReplyKeyboardButton(context, node.Previous?.Step, node.Next?.Step, cancellationToken);
            }

            if (handler is IStep<TContext> step)
            {
                node.Step = (context, cancellationToken) =>
                    step.SendStepInformationAsync(context, cancellationToken);
            }

            if (handler is IUpdateHandler<TContext> updateHandler)
            {
                node.Handler = (context, cancellationToken) =>
                    updateHandler.HandleAsync(context, node.Previous?.Step, node.Next?.Step, cancellationToken);
            }
        }

        public void SeparateObjectToHandlers(
                LinkedNode<TContext> node, 
                CallbackUpdateDelegate<TContext> callbackButtonHandler,
                ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
                HandlerDelegate<TContext> updateHandler,
                StepDelegate<TContext> stepHandler)
        {
            if (callbackButtonHandler is not null)
            {
                node.CallbackButtonHandler = 
                    (context, cancellationToken) => 
                        callbackButtonHandler(node.Previous?.Step, node.Next?.Step, context, cancellationToken);
            }

            if (replyKeyboardButtonHandler is not null)
            {
                node.ReplyKeyboardButtonHandler =
                    (context, cancellationToken) =>
                        replyKeyboardButtonHandler(node.Previous?.Step, node.Next?.Step, context, cancellationToken);
            }

            if (updateHandler is not null)
            {
                node.Handler =
                    (context, cancellationToken) =>
                        updateHandler(node.Previous?.Step, node.Next?.Step, context, cancellationToken);
            }

            if (stepHandler is not null)
            {
                node.Step =
                    (context, cancellationToken) =>
                        stepHandler(node.Previous?.Step, node.Next?.Step, context, cancellationToken);
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

        public LinkedStateMachine<TContext> Use<THandler>(
               THandler handler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
           where THandler : IUpdateHandler<TContext>
        {
            LinkedNode<TContext> newNode = new();

            SeparateObjectToHandlers(newNode, handler);
            newNode.Data = GetExecutionSequence(executionSequence)(newNode);
            AppendNode(newNode);

            return this;
        }

        public LinkedStateMachine<TContext> Use(
               CallbackUpdateDelegate<TContext> callbackButtonHandler,
               ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
               HandlerDelegate<TContext> updateHandler,
               StepDelegate<TContext> stepHandler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null)
        {
            LinkedNode<TContext> newNode = new();

            SeparateObjectToHandlers(newNode, callbackButtonHandler, replyKeyboardButtonHandler, updateHandler, stepHandler);
            newNode.Data = GetExecutionSequence(executionSequence)(newNode);
            AppendNode(newNode);

            return this;
        }
    }
}
