using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.Form.v3.Elements
{
    public abstract class BaseElement<TContext> where TContext : IUpdateContext
    {
        public int Step { get; set; }
        public int HandlerStep => Step + 1;
        public string PropertyName { get; set; }

        public virtual Predicate<TContext> StepPredicate => (context) =>
            (context.UserState.CurrentState.Step == Step || context.UserState.CurrentState.Step == HandlerStep);

        public virtual UpdateDelegate<TContext> GetExecuteSequence(LinkedNode<TContext> node)
        {
            return async (context, cancellationToken) =>
            {
                if (StepPredicate(context))
                {
                    if (context.UserState?.CurrentState.Step == Step)
                    {
                        await node.Step(context);
                    }
                    else if (context.UserState?.CurrentState.Step == HandlerStep)
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
    }
}
