using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JutsuBot.Elements.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FormModel> Forms { get; set; }
        public DbSet<FormPropertyMetadata> FormProperties { get; set; }
        public DbSet<MessageLocalization> LocalizationMessages { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<TrackedMessage> TrackedMessages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User
            var user = modelBuilder.Entity<User>();
            user.HasKey(u => u.Id);
            user.Property(u => u.Id).ValueGeneratedNever();

            user
                .HasOne(u => u.CurrentState)
                .WithOne(u => u.UserForCurrentState)
                .HasForeignKey<State>(s => s.CurrentStateUserId)
                .OnDelete(DeleteBehavior.NoAction);

            user
                .HasMany(u => u.MessageState)
                .WithOne(u => u.MessageStateUser)
                .HasForeignKey(s => s.MessageStateUserId)
                .OnDelete(DeleteBehavior.NoAction);

            //State
            var state = modelBuilder.Entity<State>();
            state.HasKey(s => s.StateId);

            state
                .HasOne(s => s.Message)
                .WithOne(t => t.State)
                .HasForeignKey<TrackedMessage>(t => t.StateId)
                .OnDelete(DeleteBehavior.NoAction);

            //Form
            var form = modelBuilder.Entity<FormModel>();
            form.HasKey(f => f.FormId);

            form.HasOne(f => f.FormInformationMessage)
                .WithOne(t => t.InformationMessageForm)
                .HasForeignKey<TrackedMessage>(t => t.InformationMessageFormId)
                .OnDelete(DeleteBehavior.NoAction);

            form.HasMany(f => f.FormUtilityMessages)
                .WithOne(t => t.UtilityMessageForm)
                .HasForeignKey(t => t.UtilityMessageFormId)
                .OnDelete(DeleteBehavior.NoAction);

            form.HasMany(f => f.FormProperties)
                .WithOne(p => p.Form)
                .HasForeignKey(f => f.FormId)
                .OnDelete(DeleteBehavior.NoAction);

            //Form property
            var formProperty = modelBuilder.Entity<FormPropertyMetadata>();
            formProperty.HasKey(f => f.Id);

            //Message localization
            var messageLocalization = modelBuilder.Entity<MessageLocalization>();
            messageLocalization.HasKey(m => m.Id);

            var trackedMessage = modelBuilder.Entity<TrackedMessage>();
            trackedMessage.HasKey(t => t.MessageId);
            trackedMessage.Property(t => t.MessageId).ValueGeneratedNever();
        }
    }
}
