using Jutsu.Telegarm.Bot.UpdatePipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.PipelineComponents
{
    public class DefaultNavigationConfigurator<TContext> : INavigationConfigurator<TContext>
        where TContext : IUpdateContext
    {
        public UpdateDelegate<TContext> ExtendedNextDelegate(LinkedNode<TContext> node)
        {
            return async (context, cancellationToken) =>
            {
                context.UserState.CurrentState.Step++;
                await node.Next?.Data(context, cancellationToken);
            };
        }

        public UpdateDelegate<TContext> ExtendedPrevDelegate(LinkedNode<TContext> node)
        {
            return async (context, cancellationToken) =>
            {
                context.UserState.CurrentState.Step -= 3;
                await node.Previous?.Data(context, cancellationToken);
            };
        }
    }
}
