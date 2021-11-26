using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace Htlv.Parser.Pagination.Models
{
    public class SelectorInfo<T, TContext> where TContext : IUpdateContext
    {
        public Func<TContext, Task<string>> InformationText { get; set; }
        public Func<TContext, Task<IQueryable<T>>> DataConfigurator { get; set; }
        public Func<T, string> CallbackDataTemplate { get; set; }
        public Func<T, string> InlineButtonTextTemplate { get; set; }
        public HandlerDelegate<TContext> HandleSelectedItem { get; set; }
        public List<List<InlineKeyboardButton>> NavigationButtons { get; set; }
        public List<List<InlineKeyboardButton>> WorkerButtons { get; set; }
        public CallbackUpdateDelegate<TContext> NavigationButtonsHandler { get; set; }
        public Action<PaginationInfo> PaginationConfigurator { get; set; }
        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> ExtendedPrevDelegate { get; set; }
        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> ExtendedNextDelegate { get; set; }
        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> ExecutionSequence { get; set; }
    }
}
