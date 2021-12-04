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
using TgBotFramework;
using CliverBot.Console.Extensions;

namespace CliverBot.Console.Form.Authorization.Handlers
{
    public class FirstNameStep : IStep<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        private readonly FormRepository _formRepository;
        private readonly MessageLocalizationRepository _messageLocalization;

        public FirstNameStep(FormRepository formRepository, MessageLocalizationRepository messageLocalization)
        {
            _formRepository = formRepository;
            _messageLocalization = messageLocalization;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context))
            {
                var formId = context.UserState.CurrentState.Stage.GetFormId();
                var form = _formRepository.GetFormById(formId);

                context.UserState.CurrentState.CacheData = 
                    context.UserState.CurrentState.CacheData.AddProperty<AuthorizationModel>(context.Update.Message.Text, nameof(AuthorizationModel.FirstName));

                form.FormUtilityMessages.Add(new TrackedMessage() { ChatId = context.Update.GetSenderId(), MessageId = context.Update.Message.MessageId });

                StringBuilder messageBuilder = new();

                messageBuilder.AppendLine($"<b>{_messageLocalization.GetMessage("authorization.form")}</b>");
                messageBuilder.AppendLine($"<b>{_messageLocalization.GetMessage("authorization.form.firstName")}</b>: {context.Update.Message.Text}");

                await context.Client.EditMessageTextAsync(context.Update.GetSenderId(), form.FormInformationMessage.MessageId,
                    text: messageBuilder.ToString());

                foreach(var utilityMessage in form.FormUtilityMessages)
                {
                    await context.Client.DeleteMessageAsync(utilityMessage.ChatId, utilityMessage.MessageId);
                }

                form.FormUtilityMessages.Clear();
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var formId = context.UserState.CurrentState.Stage.GetFormId();
            var form = _formRepository.GetFormById(formId);

            var message = await context.Client.SendTextMessageAsync(context.Update.GetSenderId(),
                $"<b>{_messageLocalization.GetMessage("authorization.form.firstName.help.add")}", ParseMode.Html);

            form.FormUtilityMessages.Add(new TrackedMessage() { ChatId = context.Update.GetSenderId(), MessageId = message.MessageId });

            StringBuilder messageBuilder = new();

            messageBuilder.AppendLine($"<b>{_messageLocalization.GetMessage("authorization.form")}</b>");
            messageBuilder.AppendLine($"<b>{_messageLocalization.GetMessage("authorization.form.firstName")}</b>: {_messageLocalization.GetMessage("authorization.form.firstName.placeholder")}");

            await context.Client.EditMessageTextAsync(context.Update.GetSenderId(), form.FormInformationMessage.MessageId,
                text: messageBuilder.ToString());

            context.UserState.CurrentState.Step++;
        }
    }
}
