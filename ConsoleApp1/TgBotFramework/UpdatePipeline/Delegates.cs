using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public delegate Task<bool> UpdateButtonDelegate<TContext>(TContext context, CancellationToken cancellationToken = default) where TContext : IUpdateContext;

    public delegate Task<bool> CallbackUpdateDelegate<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
        where TContext : IUpdateContext;

    public delegate Task<bool> ReplyUpdateDelegate<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
       where TContext : IUpdateContext; 

    public delegate Task StepDelegate<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
       where TContext : IUpdateContext;

    public delegate Task HandlerDelegate<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
       where TContext : IUpdateContext;

    //public delegate Task UpdateDelegate<TContext>(UpdateDelegate<TContext>, UpdateDelegate<TContext>, TContext, CancellationToken)
    //   where TContext : IUpdateContext;
}
