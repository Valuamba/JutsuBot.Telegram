using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.Interfaces
{
    public interface IStep<TContext> where TContext : IUpdateContext
    {
        Task NotifyStep(TContext context, CancellationToken cancellationToken);
    }

}
