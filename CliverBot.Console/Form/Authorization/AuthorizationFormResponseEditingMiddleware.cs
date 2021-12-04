using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Form.Authorization
{
    public class AuthorizationFormResponseEditingMiddleware<TContext> : IUpdateHandler<TContext>
        where TContext : IUpdateContext
    {
        private readonly MessageRepository _messageRepository;
        private readonly FormRepository _formRepository;

        public AuthorizationFormResponseEditingMiddleware(
            MessageRepository messageRepository,
            FormRepository formRepository)
        {
            _messageRepository = messageRepository;
            _formRepository = formRepository;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            await next(context, cancellationToken);
            //context.UserState.CurrentState.Stage.GetParameter("fornId")
            int formId = context.UserState;
            var formModel = _formRepository.GetFormById(formId);

            context.Client.EditMessageTextAsync(context.Update.GetSenderId(), formModel.FormInformationMessage.MessageId, );

            _messageRepository.DeleteMessages(context.Update.GetSenderId(), MessageType.BeDeleted);
        }
    }
}
