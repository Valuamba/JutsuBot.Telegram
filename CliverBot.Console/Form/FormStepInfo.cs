using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.Form
{
    public delegate Task StandartHandler<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken = default) where TContext : IUpdateContext;

    public class FormStepInfo<TContext> where TContext : IUpdateContext
    {
        public List<ValidationHandler<TContext>> ValidationHandlers { get; set; }
        public int Step { get; set; }
        public string Stage { get; set; }
        public string PropertyName { get; set; }
        public string InformationText { get; set; }
        public string ErrorText { get; set; }
        public InlineKeyboardMarkup EntryInlineKeyboardMarkup { get; set; }
        public ReplyKeyboardMarkup EntryReplyKeyboardMarkup { get; set; }
        public ReplyUpdateDelegate<TContext> ReplyKeyboardHandler { get; set; }
        public CallbackUpdateDelegate<TContext> CallbackKeyboardHandler { get; set; }


        //UsersNotifier = new UsersNotifier(options =>

        //    options.InfoRoles = new[] { Role.Admin, Role.Moderator },
        //    options.Type = NotifyType.Accept,
        //    options.ResponsibleRole = Role.Admin,

        //)
    }
}
