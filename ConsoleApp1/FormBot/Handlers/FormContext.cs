using ConsoleApp1;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace JutsuForms.Server.FormBot.Handlers
{
    public class FormContext
    {
        private List<FormHandlerContext> _formHandlersContext = new List<FormHandlerContext>();

        public string FormName = "Authorization Form";

        public void AddFormHandlerContext(FormHandlerContext formHandlerContext)
        {
            _formHandlersContext.Add(formHandlerContext);
        }

        public List<FormHandlerContext> FormHandlersContext
            => _formHandlersContext;
    }

    public class FormHandlerContext
    {
        public Type FieldType { get; set; }
        public int Step { get; set; }
        public string Stage { get; set; }
        public string FieldName { get; set; }
        public string Placeholder { get; set; }
        public string InformationMessage { get; set; }
        public ReplyKeyboardMarkup NavigationReplyMarkup { get; set; }
        public List<ValidationHandler<BotExampleContext>> ValidationHandler { get; set; }
        public Func<FormService, BotExampleContext, int, UpdateDelegate<BotExampleContext>, UpdateDelegate<BotExampleContext>, CancellationToken, Task<bool>> NavigationButtonHandler { get; set; }
    }
}
