using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Repositories;
using ConsoleApp1.BackgroundServiceData;
using ConsoleApp1.FormBot.Handlers;
using Htlv.Parser.DataAccess.EF;
using JutsuBot.Elements.DataAccess;
using JutsuBot.Elements.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TgBotFramework;
using ConsoleApp1.FormBot;
using ConsoleApp1.FormBot.Handlers.Menu;

namespace ConsoleApp1
{
    class Program
    {
        readonly static CancellationTokenSource _cancelTokenSrc = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    IConfiguration config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    services.AddLogging();
                    //services.Configure<BotSettings>(config.GetSection(nameof(BaseBot)));

                    //services.AddSingleton<MonitorLoop>();
                    //services.AddHostedService<QueuedHostedService>();
                    //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

                    //services.AddSingleton(Channel.CreateUnbounded<string>(
                    //    new UnboundedChannelOptions()
                    //    {
                    //        SingleWriter = true
                    //    })
                    //);

                    services.AddScoped<SomeHandler>();
                    services.AddScoped<MenuStep>();
                    services.AddScoped<AuthorizationNameHandler>();
                    services.AddScoped<AuthorizationAgeHandler>();
                    services.AddScoped<AuthorizationValidation>();
                    services.AddScoped<AuthorizationConfirmation>();
                    services.AddScoped<UserStateMapperMiddleware<BotExampleContext>>();
                    services.AddScoped<StateMapperMiddleware<BotExampleContext>>();
                    services.AddScoped<StateRepository>();
                    services.AddScoped<FormPropertyMetadataRepository>();
                    services.AddScoped<MemoryRepository>();
                    services.AddScoped<MessageLocalizationRepository>();
                    services.AddScoped<UserRepository>();
                    services.AddScoped<TrackedMessageRepository>();

                    services.AddDbContext<ApplicationDbContext>(options => 
                        options.UseSqlServer("Data Source=.\\SqlExpress;Initial Catalog=TestStudyDb;Integrated Security=True"));
                    //services.AddHostedService<UpdateHandler>();
                    //services.AddHostedService<WorkerHostedService>();

                    services.AddBotService<BaseBot, BotExampleContext>(x => x
                        //select way/settings for getting update
                        .UseLongPolling<PollingManager<BotExampleContext>>()

                        .SetPipeline(pipelineBuilder => pipelineBuilder

                            .UseMiddleware<UserStateMapperMiddleware<BotExampleContext>>()
                            .UseMiddleware<StateMapperMiddleware<BotExampleContext>>()

                            .Stage("menu", branch => branch 
                                .Step<MenuStep>()
                            )

                            .Stage("authorization", branch => branch
                                .Step<AuthorizationNameHandler>(0)
                                .Step<AuthorizationAgeHandler>(2)
                                .Handler<AuthorizationValidation>(4)
                                .Handler<AuthorizationConfirmation>(5)
                            )
                        )
                    );

                }).RunConsoleAsync();

            //var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
            //monitorLoop.StartMonitorLoop();

            //var botService = host.Services.GetRequiredService<BotService<BaseBot, BotExampleContext>>();
            //await botService.StartAsync(CancellationToken.None);

            //await host.WaitForShutdownAsync();
        }
    }

    public class CustomHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }

    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
            CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public BackgroundTaskQueue(int capacity = 100)
        {
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(
            Func<CancellationToken, ValueTask> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }

    public class MonitorLoop
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger _logger;
        private readonly CancellationToken _cancellationToken;

        public MonitorLoop(IBackgroundTaskQueue taskQueue,
            ILogger<MonitorLoop> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void StartMonitorLoop()
        {
            _logger.LogInformation("MonitorAsync Loop is starting.");

            // Run a console user input loop in a background thread
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var keyStroke = Console.ReadKey();

                if (keyStroke.Key == ConsoleKey.W)
                {
                    // Enqueue a background work item
                    await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
                }
            }
        }

        private async ValueTask BuildWorkItem(CancellationToken token)
        {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item

            int delayLoop = 0;
            var guid = Guid.NewGuid().ToString();

            _logger.LogInformation("Queued Background Task {Guid} is starting.", guid);

            while (!token.IsCancellationRequested && delayLoop < 3)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if the Delay is cancelled
                }

                delayLoop++;

                _logger.LogInformation("Queued Background Task {Guid} is running. "
                                       + "{DelayLoop}/3", guid, delayLoop);
            }

            if (delayLoop == 3)
            {
                _logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
            }
            else
            {
                _logger.LogInformation("Queued Background Task {Guid} was cancelled.", guid);
            }
        }
    }

    //public class CustomHostedService : BackgroundService
}
