using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework
{
    /// <summary>
    /// Processes an update
    /// </summary>
    public interface ICallbackButtonHandler<TContext> where TContext : IUpdateContext
    {
        Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken) ;
    }
}