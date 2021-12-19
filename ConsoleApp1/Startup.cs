using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Repositories;
using ConsoleApp1;
using ConsoleApp1.FormBot.Handlers;
using ConsoleApp1.FormBot.Handlers.Menu;
using Htlv.Parser.DataAccess.EF;
using JutsuBot.Elements.DataAccess;
using JutsuBot.Elements.DataAccess.Repositories;
using JutsuForms.Server.TgBotFramework.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBotFramework;
using ConsoleApp1.FormBot;
using JutsuForms.Server.TgBotFramework;
using JutsuForms.Server.FormBot.Predicates;
using JutsuForms.Server.FormBot.Handlers;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.FormBot;
using ConsoleApp1.FormBot.Models;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;
using ConsoleApp1.FormBot.Extensions;
using JutsuForms.Server.FormBot.Handlers.Authorization.Handlers;
using JutsuForms.Server.FormBot.Models;
using JutsuForms.Server.FormBot.Middlewares;

namespace JutsuForms.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotSettings>>();
                return new TelegramBotClient(options.Value.ApiToken, baseUrl: options.Value.BaseUrl);
            });

            services.AddSignalR();
            services.AddLogging();
            services.AddScoped<IUpdateService, TelegramUpdateService>();
            services.AddScoped<TrackedMessageRepository>();
            services.AddScoped<FormService>();
            services.Configure<BotSettings>(Configuration.GetSection(nameof(BaseBot)));

            services.AddSingleton<FormContext>();

            services.AddScoped<MenuStep>();
            services.AddScoped<AuthorizationFormEnterHandler>();
            services.AddScoped<GlobalExceptionHandlerMiddleware>();
            //services.AddScoped<AuthorizationNameHandler>();
            services.AddScoped<AuthorizationEnding>();
            services.AddScoped<ResolveAuthorizationHandler>();
            //services.AddScoped<AuthorizationAgeHandler>();
            services.AddScoped<AuthorizationValidation>();
            services.AddScoped<AuthorizationConfirmationRequest>();
            services.AddScoped<UserStateMapperMiddleware<BotExampleContext>>();
            services.AddScoped<StateMapperMiddleware<BotExampleContext>>();
            services.AddScoped<StateRepository>();
            services.AddScoped<FormPropertyMetadataRepository>();
            services.AddScoped<MemoryRepository>();
            services.AddScoped<MessageLocalizationRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<TrackedMessageRepository>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Data Source=DESKTOP-QEJJ1L4;Initial Catalog=JutsuForm;Integrated Security=True"));

            services.AddBotService<BaseBot, BotExampleContext>(x => x
                .UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions())

                .SetPipeline(pipelineBuilder => pipelineBuilder

                    .UseMiddleware<GlobalExceptionHandlerMiddleware>()
                    .UseMiddleware<UserStateMapperMiddleware<BotExampleContext>>()
                    .UseMiddleware<StateMapperMiddleware<BotExampleContext>>()

                    .Stage("menu", branch => branch
                        .Step<MenuStep>()
                    )
                    .Form("authorization", branch => branch
                        .When(IsRole.Visitor, branch => branch
                            .Handler<AuthorizationFormEnterHandler>(0)
                            .FormStep<AuthorizationAgeHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 1,
                                FieldType = typeof(int),
                                FieldName = nameof(AuthorizationModels.Age),
                                InformationMessage = "Write your age",
                                Placeholder = "...✍️",
                                ValidationHandler = new List<ValidationHandler<BotExampleContext>>()
                                {
                                    new ValidationHandler<BotExampleContext>()
                                    {
                                        ErrorMessageAlias = "You should write number.",
                                        UpdatePredicate = (context) => new Regex("\\d+").IsMatch(context.Update.Message.Text)
                                    }
                                },
                                NavigationReplyMarkup = new string[][] { 
                                    new [] { "Cancel" }
                                },
                            })
                            .FormStep<InterestHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 3,
                                FieldType = typeof(List<InterestType>),
                                FieldName = nameof(AuthorizationModels.Interests),
                                InformationMessage = "Select your interests. This information will used in creting requerements for you.",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .FormStep<CountHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 5,
                                FieldType = typeof(int),
                                FieldName = nameof(AuthorizationModels.Count),
                                InformationMessage = "Write count",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .FormStep<AddressHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 7,
                                FieldType = typeof(string),
                                FieldName = nameof(AuthorizationModels.Address),
                                InformationMessage = "Write address",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .FormStep<EmailHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 9,
                                FieldType = typeof(string),
                                FieldName = nameof(AuthorizationModels.Email),
                                InformationMessage = "Write email",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .FormStep<MMRHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 11,
                                FieldType = typeof(string),
                                FieldName = nameof(AuthorizationModels.MMR),
                                InformationMessage = "Write MMR",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .FormStep<MMRHandler>(formHandlerContext: new FormHandlerContext()
                            {
                                Step = 13,
                                FieldType = typeof(int),
                                FieldName = nameof(AuthorizationModels.Index),
                                InformationMessage = "Write index",
                                Placeholder = "...✍️",
                                NavigationReplyMarkup = new string[][] {
                                    new [] { "Back" },
                                    new [] { "Cancel" },
                                }
                            })
                            .Handler<AuthorizationValidation>(5)
                            .Step<AuthorizationConfirmationRequest>(6)
                            .Handler<AuthorizationEnding>(8)
                        )
                        .When(IsRole.Admin, branch => branch
                            .Handler<ResolveAuthorizationHandler>(0)
                        )
                    )
                )
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UpdateHub>("/update");
            });
        }
    }
}
