using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using CliverBot.Console.Handlers;
using CliverBot.Console.DataAccess;
using CliverBot.Console.Form.Authorization;
using Htlv.Parser.DataAccess.EF;

namespace CliverBot.Console
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            #pragma warning disable CA1416 // Validate platform compatibility

            await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    IConfiguration config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    services.AddLogging();
                    services.Configure<BotSettings>(config.GetSection(nameof(EchoBot)));

                    services.AddScoped<UserStateMapperMiddleware<BotExampleContext>>();
                    services.AddScoped<StateMapperMiddleware<BotExampleContext>>();
                    services.AddScoped<MenuHandler>();
                    services.AddScoped<ConfirmAuthorization>();
                    services.AddSingleton<MemoryRepository>();
                    services.AddSingleton<StateRepository>();

                    MemoryRepository memoryRepository = new();

                    services.AddBotService<EchoBot, BotExampleContext>(x => x

                        .UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions())

                        .SetPipeline(pipelineBuilder => pipelineBuilder

                            .Step<UserStateMapperMiddleware<BotExampleContext>>(executionSequence: (node) => node.Handler)
                            .Step<StateMapperMiddleware<BotExampleContext>>(executionSequence: (node) => node.Handler)

                            .Stage("Authorization", branch => branch
                                .CreateAuthPipeline()
                            )
                            .Stage("confirmAuthorization", branch => branch
                                .Step<ConfirmAuthorization>()
                            )
                            
                            .Stage("menu", branch => branch
                                .Step<MenuHandler>()
                            )
                        )
                    );

                }).RunConsoleAsync();

                #pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
