using ConsoleApp1;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;

namespace JutsuForms.Server.FormBot.Middlewares
{
    public class GlobalExceptionHandlerMiddleware : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
                _logger.LogInformation("Update {0}, no errors", context.Update.Id);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Update {0}, has errors {1}", context.Update.Id, e);
            }
        }
    }
}
