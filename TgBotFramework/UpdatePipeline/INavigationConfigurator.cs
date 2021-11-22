using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace Jutsu.Telegarm.Bot.UpdatePipeline
{
    public interface INavigationConfigurator<TContext> where TContext : IUpdateContext
    {
        UpdateDelegate<TContext> ExtendedNextDelegate(TgBotFramework.UpdatePipeline.LinkedNode<TContext> node);
        UpdateDelegate<TContext> ExtendedPrevDelegate(TgBotFramework.UpdatePipeline.LinkedNode<TContext> node);
    }
}
