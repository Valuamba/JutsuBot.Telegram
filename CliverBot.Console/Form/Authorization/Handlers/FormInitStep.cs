using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;
using Telegram.Bot.Types.Enums;

namespace CliverBot.Console.Form.Authorization.Handlers
{
    public class FormInitStep : IStep<BotExampleContext>
    {
        private readonly FormRepository _formRepository;
        private readonly MessageLocalizationRepository _messageLocalization;

        public FormInitStep(FormRepository formRepository, MessageLocalizationRepository messageLocalization)
        {
            _formRepository = formRepository;
            _messageLocalization = messageLocalization;
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var form = _formRepository.AddForm(new FormModel());
            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), 
                $"<b>{_messageLocalization.GetMessage("authorization.form")}</b> \r\n\r\n", ParseMode.Html);
            context.UserState.CurrentState.Stage = $"authorization/form_id={form.FormId}";
            context.UserState.CurrentState.Step += 2;
        }
    }
}
