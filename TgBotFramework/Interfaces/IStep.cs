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
        Task SendStepInformationAsync(TContext context, CancellationToken cancellationToken);
    }

}
