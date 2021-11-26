using Htlv.Parser.DataAccess.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Htlv.Parser
{
    public class BotFrameworkContextFactory : IDesignTimeDbContextFactory<BotDbContext>
    {
        public BotDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("SqlServerConnection"),
                x => x.MigrationsAssembly(typeof(BotDbContext).Assembly.FullName));

            return new BotDbContext(optionsBuilder.Options);
        }
    }
}
