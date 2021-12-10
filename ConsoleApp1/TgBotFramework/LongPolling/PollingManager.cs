using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PollingManager<TContext> : BackgroundService, IPollingManager<TContext>
        where TContext : IUpdateContext
    {
        private readonly ILogger<PollingManager<TContext>> _logger;
        private readonly LongPollingOptions _pollingOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelWriter<IUpdateContext> _channel;

        public PollingManager(
            ILogger<PollingManager<TContext>> logger, 
            Channel<IUpdateContext> channel,
            IServiceProvider serviceProvider) 
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channel = channel.Writer;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consoleMessage = Console.ReadLine();

                    var updateContext = _serviceProvider.GetService<IUpdateContext>();

                    Debug.Assert(updateContext != null, nameof(updateContext) + " != null");

                    updateContext.UserState = new UserState()
                    {
                        CurrentState = new State()
                        {
                            Stage = "default"
                        }
                    };
                    updateContext.Update = new Update()
                    {
                        Message = new Message()
                        {
                            Text = consoleMessage,
                            From = new User()
                            {
                                Id = 123
                            }
                        }
                    };

                    await _channel.WriteAsync(updateContext, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while polling in " + nameof(PollingManager<TContext>));
                }
            }
        }

        private async ValueTask SendLogger(ITelegramBotClient botclient, ApiRequestEventArgs args, CancellationToken cancellationtoken)
        {
            _logger.LogInformation("Sending method {0}, content:\n\t{1}",
                args.MethodName,
                await  args.HttpContent.ReadAsStringAsync());
        }

        private async ValueTask ReceiveLogger(ITelegramBotClient client, ApiResponseEventArgs args, CancellationToken token)
        {
            _logger.LogInformation("Received response, method {0}, code {1} content:\n\t{2}",
                args.ApiRequestEventArgs.MethodName,
                args.ResponseMessage.StatusCode,
                await args.ResponseMessage.Content.ReadAsStringAsync(token));
        }
    }

    public interface IPollingManager<in TContext> 
        where TContext : IUpdateContext
    {
    }
}