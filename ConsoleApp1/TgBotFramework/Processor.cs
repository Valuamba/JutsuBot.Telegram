using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Jutsu.Telegarm.Bot.Models;
using JutsuForms.Server.TgBotFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework.Exceptions;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework
{
    public class BotService<TBot, TContext> : BackgroundService  
        where TBot : BaseBot
        where TContext : IUpdateContext 
    {
        private readonly ILogger<BotService<TBot, TContext>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TBot _bot;
        private readonly ChannelReader<IUpdateContext> _updatesQueue;
        private readonly UpdateDelegate<TContext> _updateHandler;

        public BotService(ILogger<BotService<TBot, TContext>> logger,
            IServiceProvider serviceProvider,
            Channel<IUpdateContext> updatesQueue, 
            TBot bot, 
            UpdatePipelineSettings<TContext> updatePipelineSettings
            ) 
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _bot = bot;
            _updatesQueue = updatesQueue.Reader;

            var pipe = new LinkedStateMachine<TContext>(new());

            updatePipelineSettings.PipeSettings(pipe);

            //TODO:
            CheckPipeline(pipe, serviceProvider);

            _updateHandler = pipe.Head?.Data;
        }

        private void CheckPipeline(LinkedStateMachine<TContext> pipe, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            foreach (var type in pipe.ServiceCollection)
            {
                Type typeToResolve = type.ImplementationType;
                if (type.ServiceType.IsGenericTypeDefinition)
                {
                    typeToResolve = type.ServiceType.MakeGenericType(typeof(TContext));
                }

                if (scope.ServiceProvider.GetService(typeToResolve) == null)
                {
                    _logger.LogCritical("There is no service type of {0} in DI", typeToResolve.FullName);
                    throw new PipelineException(
                        string.Format("There is no service type of {0} in DI", typeToResolve.FullName));
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await foreach (var update in _updatesQueue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    update.Services = scope.ServiceProvider;
                    update.Bot = _bot;
                    update.StageContext = new StageContext();
                    update.BotClient = scope.ServiceProvider.GetRequiredService<IUpdateService>();

                    await _updateHandler((TContext) update, stoppingToken);

                    if (update.Result != null)
                    {
                        //TODO: callback on finish
                        Task.Run(() => update.Result.TrySetResult());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Oops");
                    
                }
            }
        }
    }
}