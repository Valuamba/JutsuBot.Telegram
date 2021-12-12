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
using Microsoft.AspNetCore.Hosting;

namespace JutsuForms.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
