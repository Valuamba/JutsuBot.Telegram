using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public interface ILinkedStateMachine<TContext> where TContext : IUpdateContext
    {
        LinkedNode<TContext> Head { get; }
        ILinkedStateMachine<TContext> Stage(string stage, Action<ILinkedStateMachine<TContext>> branch);
        Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecutionSequence(Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);
        void SeparateObjectToHandlers<THandler>(
            LinkedNode<TContext> node,
            THandler handler);
        void SeparateObjectToHandlers(LinkedNode<TContext> node, CallbackUpdateDelegate<TContext> callbackButtonHandler, ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler, HandlerDelegate<TContext> updateHandler, StepDelegate<TContext> stepHandler);
        void AppendNode(LinkedNode<TContext> node);
        ILinkedStateMachine<TContext> Use<THandler>(THandler handler, Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);
        ILinkedStateMachine<TContext> Use(CallbackUpdateDelegate<TContext> callbackButtonHandler, ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler, HandlerDelegate<TContext> updateHandler, StepDelegate<TContext> stepHandler, Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);
    }
}
