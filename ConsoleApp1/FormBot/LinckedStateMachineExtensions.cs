using ConsoleApp1.TgBotFramework.UpdatePipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace ConsoleApp1.FormBot
{
    public static class LinckedStateMachineExtensions
    {
        public static ILinkedStateMachine<BotExampleContext> UseMiddleware<THandler>(this ILinkedStateMachine<BotExampleContext> pipe)
        {
            pipe.Step<THandler>(executionSequence: (node) => async (context, cancellationToken) => await node.Handler(context));
            return pipe;
        }

        public static ILinkedStateMachine<BotExampleContext> When(this ILinkedStateMachine<BotExampleContext> pipe, Predicate<BotExampleContext> predicate, Action<ILinkedStateMachine<BotExampleContext>> branch)
        {
            var whenBranch = new LinkedStateMachine<BotExampleContext>(pipe.ServiceCollection);
            branch(whenBranch);

            LinkedNode<BotExampleContext> newNode = new();

            newNode.Data = (context, cancellationToken) =>
                predicate(context)
                ? whenBranch.Head.Data(context, cancellationToken)
                : newNode.Next.Data(context, cancellationToken);

            pipe.AppendNode(newNode);

            return pipe;
        }

        public static ILinkedStateMachine<BotExampleContext> Handler<THandler>(this ILinkedStateMachine<BotExampleContext> pipe, int handlerStepNumber)
            where THandler : IUpdateHandler<BotExampleContext>
        {
            pipe.Step<THandler>(executionSequence: GetHandlerExecutionSequence(handlerStepNumber));
            return pipe;
        }

        public static ILinkedStateMachine<BotExampleContext> Step<THandler>(this ILinkedStateMachine<BotExampleContext> pipe, int notifyMessageStep)
        {
            pipe.Step<THandler>(executionSequence: GetStepExecutionSequence(notifyMessageStep));
            return pipe;
        }

        public static Func<LinkedNode<BotExampleContext>, UpdateDelegate<BotExampleContext>> GetHandlerExecutionSequence(int handlerStep)
               => (node) => async (context, cancellationToken) =>
               {
                   if (context.UserState?.CurrentState.Step == handlerStep)
                   {
                       await node.Handler(context);
                   }
                   else
                   {
                       await node.Next.Data(context, cancellationToken);
                   }
               };

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
    }

    public enum StepType
    {
        NotifyMessage = 0,
        UpdateHandler = 1
    }
}
