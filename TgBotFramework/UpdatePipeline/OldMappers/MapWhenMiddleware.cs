using System;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline.OldMappers
{
    public class MapWhenMiddleware<TContext> : ICallbackButtonHandler<TContext> where TContext : IUpdateContext
    {
        private readonly Predicate<TContext> _predicate;

        private readonly UpdateDelegate<TContext> _branch;

        public MapWhenMiddleware(Predicate<TContext> predicate, UpdateDelegate<TContext> branch)
        {
            _predicate = predicate;
            _branch = branch;
        }

        public Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
            => _predicate(context)
                ? _branch(context, cancellationToken)
                : next(context, cancellationToken);

        public Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}