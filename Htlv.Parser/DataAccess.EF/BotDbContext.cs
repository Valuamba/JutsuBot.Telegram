using Htlv.Parser.DataAccess.EF.Entities;
using Htlv.Parser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.DataAccess.EF
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }

        public DbSet<CSGOMatch> CSGOMatches { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var csgoMatchBuild = modelBuilder.Entity<CSGOMatch>();

            csgoMatchBuild.HasKey(p => p.Id);

            var user = modelBuilder.Entity<User>();
            user.HasKey(u => u.Id);
            user.Property(u => u.Id).ValueGeneratedNever();

            user
                .HasOne(u => u.CurrentState)
                .WithOne(u => u.UserForCurrentState)
                .HasForeignKey<State>(s => s.CurrentStateUserId)
                .OnDelete(DeleteBehavior.NoAction);

            user
               .HasOne(u => u.PrevState)
               .WithOne(u => u.UserForPrevState)
               .HasForeignKey<State>(s => s.PrevStateUserId)
               .OnDelete(DeleteBehavior.NoAction);

            user
               .HasMany(u => u.MessageState)
               .WithOne(u => u.UserForMessageState)
               .HasForeignKey(s => s.MessageStateUserId)
               .OnDelete(DeleteBehavior.NoAction);

            var state = modelBuilder.Entity<State>();
            state.HasKey(s => s.StateId);
        }
    }
}
