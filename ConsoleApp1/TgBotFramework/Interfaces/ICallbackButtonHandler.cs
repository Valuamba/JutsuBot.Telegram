using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.Interfaces
{
    public interface ICallbackButtonHandler<TContext> where TContext : IUpdateContext
    {
        Task<bool> HandleCallbackButton(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken);
    }
}
