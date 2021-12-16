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

                    .UseMiddleware<UserStateMapperMiddleware<BotExampleContext>>()
                    .UseMiddleware<StateMapperMiddleware<BotExampleContext>>()

                    .Stage("menu", branch => branch
                        .Step<MenuStep>()
                    )
                    .Form("authorization", branch => branch
                        //Возможно стоит сюда добавить мидл вар, который будет инициализировать пользователя с Forms?
                        //.UseMiddleware<UserFormMapperMiddleware>()
                        .When(IsRole.Visitor, branch => branch
                            .Handler<AuthorizationFormEnterHandler>(0)
                            .FormStep<AuthorizationAgeHandler>(1, nameof(AuthorizationModels.Age))
                            .FormStep<AuthorizationNameHandler>(3, nameof(AuthorizationModels.Name))
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
