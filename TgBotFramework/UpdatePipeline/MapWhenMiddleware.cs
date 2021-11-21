using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public class MapWhenMiddleware<TContext> : IUpdateHandler<TContext>
        where TContext : IUpdateContext
    {
        private readonly UpdateDelegate<TContext> _branch;

        public MapWhenMiddleware(UpdateDelegate<TContext> branch)
        {
            _branch = branch;
        }

        public Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
           => _branch(context, cancellationToken);
    }
}
