using CliverBot.Console.DataAccess.Repositories;
using CliverBot.Console.Form.Elements.Select;
using CliverBot.Console.Form.v3.Elements;
using Htlv.Parser.Pagination;
using Jutsu.Telegarm.Bot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;
using CliverBot.Console.Elements.FormInput;
using JutsuBot.Elements;

namespace CliverBot.Console.Form.Authorization
{
    public static class AuthorizationForm // : IFormBuilder<FormBotContext>
    {
        public static StepDelegate<FormBotContext> StepDelegate = async (prev, next, context, cancellationToken) =>
        {
            context.UserState.CurrentState.Step += 2;
            var message = await context.CustomClient.SendTextMessageAsync(context.Update.GetSenderId(), "Заполните профиль пользователя:");
            var formRepository = context.Services.GetRequiredService<FormRepository>();
            formRepository.AddForm(new DataAccess.FormModel()
            {
                FormName = "<b>Профиль:<b>",
                FormInformationMessage = new DataAccess.TrackedMessage()
                {
                    ChatId = context.Update.GetSenderId(),
                    MessageId = message.MessageId,
                    MessageType = DataAccess.MessageType.FormMessage
                }
            });
        };

        private static InputTextElement<AuthorizationModel, string, FormBotContext> FirstNameTextInput = new()
        {
            Step = 0,
            LocalizationSettings = new InputTextLocalizationSettings()
            {
                ChangePropertyCommandAlias = "form.change",
                AddPropertyCommandAlias = "form.add",
                AddPropertyValueTextAlias = "authorization.form.firstName.help.add",
                ChangePropertyTextAlias = "",
                NameAlias = "authorization.form.firstName",
                PlaceholderAlias = "authorization.form.firstName.validationError.long"
            },
            PropertyName = nameof(AuthorizationModel.FirstName),
            EntryReplyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton("Отмена"))
            {
                ResizeKeyboard = true
            },
            ReplyUpdateDelegate = async (prev, next, context, cancellationToken) =>
            {
                if (On.Message(context))
                {
                    if (context.Update.Message.Text == "Отмена")
                    {
                        await context.LeaveStage("menu", cancellationToken);
                        return true;
                    }
                }
                return false;
            }
        };

        //private SelectMultipleElements<AuthorizationModel, InterestType, BaseUpdateContext> SelectIntersetsStep = new()
        //{
        //    Step = 2,
        //    ItemsSupplier = (context) => Enum.GetValues<InterestType>().AsQueryable(),
        //    Pagination = new NextPrevPagination()
        //    {
        //        MaxElementsCount = 8,
        //    },

        //    PropertyName = nameof(AuthorizationModel.Interests),
        //    CallbackConvereter = (context) => Enum.Parse<InterestType>(context.Update.TrimCallbackCommand($"interest/")),

        //    CallbackDataTemplate = (context, interest) =>
        //    {
        //        //if()
        //        return $"check_interest/{interest}";
        //    },
        //    SelectedInlineButtonTextTemplate = (interest) => interest.ToString() + "✅",
        //    InlineButtonTextTemplate = (interest) => interest.ToString(),
        //    NotifyMessage = "Choose your interests!",
        //};

        public static ILinkedStateMachine<FormBotContext> BuildForm(this ILinkedStateMachine<FormBotContext> pipe)
        {
            //LinkedStateMachine<FormBotContext> pipe = new(new ServiceCollection());

            //pipe.Step<UseElementClientMiddleware<FormBotContext, BaseBot>>();
            //pipe.Step<AuthorizationFormResponseEditingMiddleware<FormBotContext>>();

            pipe.Step(FirstNameTextInput, executionSequence: FirstNameTextInput.GetExecuteSequence);

            //pipe.Step(SelectIntersetsStep, executionSequence: SelectIntersetsStep.GetExecuteSequence);

            return pipe;
        }
    }
}
