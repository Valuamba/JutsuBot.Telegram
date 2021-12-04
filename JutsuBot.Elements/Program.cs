using Htlv.Parser.DataAccess.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using TgBotFramework;
using JutsuBot.Elements.DataAccess.Repositories;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TgBotFramework.UpdatePipeline;
using CliverBot.Console.DataAccess;
using CliverBot.Console.Form.Authorization;

namespace JutsuBot.Elements
{
    class Program
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

                    //services.AddLogging();
                    services.Configure<BotSettings>(config.GetSection(nameof(BaseBot)));

                    services.AddScoped<UserStateMapperMiddleware<FormBotContext>>();
                    services.AddScoped<StateMapperMiddleware<FormBotContext>>();
                    services.AddSingleton<StateRepository>();

                    services.AddBotService<BaseBot, FormBotContext>(x => x

                        .UseLongPolling<PollingManager<FormBotContext>>(new LongPollingOptions())

                        .SetPipeline(pipelineBuilder => pipelineBuilder

                            .Step<UserStateMapperMiddleware<FormBotContext>>(executionSequence: (node) => node.Handler)
                            .Step<StateMapperMiddleware<FormBotContext>>(executionSequence: (node) => node.Handler)

                            .Stage("Authorization", branch => branch
                                .BuildForm()
                                //.CreateAuthPipeline()
                            )
                        )
                    );

                }).RunConsoleAsync();

            #pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
