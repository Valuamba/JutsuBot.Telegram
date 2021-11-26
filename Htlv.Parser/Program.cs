using CliverBot.Console;
using Htlv.Parser.DataAccess.EF;
using Htlv.Parser.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Htlv.Parser.Pagination;
using Htlv.Parser.DataAccess.EF.Repositories;
using Htlv.Parser.Steps.MatchStage;

namespace Htlv.Parser
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

                    services.AddLogging();

                    services.AddSingleton<CSGOMatchEqualityComparer>();
                    services.AddScoped<UserStateMapperMiddleware<BotExampleContext>>();
                    services.AddScoped<MatchRepository>();
                    services.AddScoped<UserRepository>();
                    services.AddScoped<MatchStep>();
                    services.AddScoped<NotifyStep>();

                    var provider = config.GetValue("Provider", "SqlServer");

                    services.AddDbContext<BotDbContext>(options =>
                    {
                        var settingsConectionString = provider switch
                        {
                            "PostgreeSql" => config.GetConnectionString("PostgreeSqlConnection"),
                            "SqlServer" => config.GetConnectionString("SqlServerConnection")
                        };

                        options = provider switch
                        {
                            "PostgreeSql" => options.UseNpgsql(settingsConectionString),

                            "SqlServer" => options.UseSqlServer(settingsConectionString),

                            _ => throw new Exception($"Unsupported provider: {provider}")
                        };

                        //options.EnableSensitiveDataLogging(true);
                    });

                    services.Configure<BotSettings>(config.GetSection(nameof(EchoBot)));

                    services.AddBotService<EchoBot, BotExampleContext>(x => x

                        .UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions())

                        .SetPipeline(pipelineBuilder => pipelineBuilder

                            .Step<UserStateMapperMiddleware<BotExampleContext>>()

                            .Stage("selectMatch", branch => branch
                                .AddMatchSelector()
                                .Step<MatchStep>(executionSequence: SomeExt.GetExecuteSequence<BotExampleContext>(2, 3))
                                .Step<NotifyStep>(executionSequence: SomeExt.GetExecuteSequence<BotExampleContext>(4, 5))
                            )
                        )
                    );

                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();

                        q.AddTrigger(configure =>
                        {
                            configure
                              .ForJob("scrapperJob")
                              .WithIdentity("srapperJobTrigger")
                              .StartNow()
                              .WithSimpleSchedule(x => x
                                  .WithIntervalInSeconds(40)
                                  .RepeatForever());
                        });

                        // Register the job, loading the schedule from configuration
                        q.AddJob<ScrapperJob>(configure =>
                        {
                            configure.WithIdentity("scrapperJob");
                        });
                    });

                    // Quartz.Extensions.Hosting hosting
                    services.AddQuartzHostedService(options =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        options.WaitForJobsToComplete = true;
                    });

                })
                .RunConsoleAsync();

#pragma warning restore CA1416 // Validate platform compatibility


            //var matches = GetCSGOMatches();


        }

        private static List<CSGOMatch> GetCSGOMatches()
        {
            HtmlDocument docc = new HtmlDocument();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.hltv.org/matches");
                request.Method = "GET";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        docc.Load(stream, Encoding.UTF8);
                    }
                }

                var upcomingMatchSections = docc.DocumentNode.SelectNodes("//*[@class='upcomingMatchesSection']");

                List<CSGOMatch> csgoMatches = new();

                foreach (var upcomingMatchSection in upcomingMatchSections)
                {
                    var matchDayText = upcomingMatchSection.SelectSingleNode(".//*[@class='matchDayHeadline']").InnerText;
                    CultureInfo cultureInfo = new("en-EN");
                    var matchDate = DateTime.ParseExact(matchDayText, "dddd - yyyy-MM-dd", cultureInfo);

                    var matches = upcomingMatchSection.SelectNodes(".//*[contains(@class,'upcomingMatch') and @team1 and @team2]");
                    if (matches != null)
                    {
                        foreach (var match in matches)
                        {
                            CSGOMatch matchModel = new();

                            matchModel.FirstTeam = match.SelectSingleNode(".//*[@class='matchTeam team1']/*[contains(@class, 'matchTeamName')]").InnerText;
                            matchModel.SecondTeam = match.SelectSingleNode(".//*[@class='matchTeam team2']/*[contains(@class, 'matchTeamName')]").InnerText;
                            matchModel.MatchMeta = match.SelectSingleNode(".//*[@class='matchMeta']").InnerText;
                            matchModel.MatchEvent = match.SelectSingleNode(".//*[contains(@class,'matchEventName')]").InnerText;
                            var matchTime = DateTime.ParseExact(match.SelectSingleNode(".//*[@class='matchTime']").InnerText, "HH:mm", cultureInfo);
                            matchModel.MatchTime = new DateTime(matchDate.Year, matchDate.Month, matchDate.Day, matchTime.Hour, matchTime.Minute, 0);

                            csgoMatches.Add(matchModel);
                        }
                    }
                }

                return csgoMatches;

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
