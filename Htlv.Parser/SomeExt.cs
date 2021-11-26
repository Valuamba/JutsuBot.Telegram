using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace Htlv.Parser
{
    public class SomeExt
    {
        public static Func<LinkedNode<TContext>, UpdateDelegate<TContext>> GetExecuteSequence<TContext>(int notifyStep, int handlerStep)
            where TContext : IUpdateContext
        {
            return (node) => async (context, cancellationToken) =>
            {
                if (context.UserState.CurrentState.Step == notifyStep || context.UserState.CurrentState.Step == handlerStep)
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
                            if (node.Handler is not null)
                            {
                                await node.Handler(context);
                            }
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
