using Microsoft.Extensions.DependencyInjection;
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
        ServiceCollection ServiceCollection { get; }
        ILinkedStateMachine<TContext> Stage(string stage, Action<ILinkedStateMachine<TContext>> branch);

        Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecutionSequence(Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);

        void SeparateObjectToHandlers<THandler>(
            LinkedNode<TContext> node,
            THandler handler,
            UpdateDelegate<TContext> prevDelegate = null,
            UpdateDelegate<TContext> nextDelegate = null);

        void SeparateObjectToHandlers(
            LinkedNode<TContext> node,
            CallbackUpdateDelegate<TContext> callbackButtonHandler,
            ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
            HandlerDelegate<TContext> updateHandler,
            StepDelegate<TContext> stepHandler,
            UpdateDelegate<TContext> prevDelegate = null,
            UpdateDelegate<TContext> nextDelegate = null);

        void AppendNode(LinkedNode<TContext> node);

        ILinkedStateMachine<TContext> Step<THandler>(
               THandler handler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedPrevDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);

        ILinkedStateMachine<TContext> Step(
               CallbackUpdateDelegate<TContext> callbackButtonHandler,
               ReplyUpdateDelegate<TContext> replyKeyboardButtonHandler,
               HandlerDelegate<TContext> updateHandler,
               StepDelegate<TContext> stepHandler,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedPrevDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null,
               Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);

        ILinkedStateMachine<TContext> Step<THandler>(
            Func<LinkedNode<TContext>, 
            UpdateDelegate<TContext>> extendedPrevDelegate = null, 
            Func<LinkedNode<TContext>, UpdateDelegate<TContext>> extendedNextDelegate = null, 
            Func<LinkedNode<TContext>, UpdateDelegate<TContext>> executionSequence = null);
    }
}
