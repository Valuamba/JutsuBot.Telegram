using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Telegram.Bot.Types.Enums;
using CliverBot.Console.Extensions;
using TgBotFramework.WrapperExtensions;
using CliverBot.Console.Form.v3.Elements;
using Htlv.Parser.Pagination;
using CliverBot.Console.DataAccess;
using CliverBot.Console.Form.Elements;
using CliverBot.Console.Form.Elements.Select;
using CliverBot.Console.Elements.FormInput;

namespace CliverBot.Console.Form.Authorization
{
    public static class AuthFormPipeline
    {
        private static SmartInputTextElement<AuthorizationModel, string, BotExampleContext> SecondNameTextInput = new()
        {
            Step = 0,
            PropertyName = nameof(AuthorizationModel.FirstName),
            EntryReplyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton("Отмена"))
            {
                ResizeKeyboard = true
            },

            LocalizationProperty = new InputTextLocalizationSettings()
            {
                PlaceholderAlias = "✍️...",
                AddPropertyCommandAlias = "Добавить имя",
                ChangePropertyCommandAlias = "Изменить имя",
                NameAlias = "Имя",
                AddPropertyValueTextAlias = "Введите ваше Имя.",
                ChangePropertyTextAlias = "Измените ваше Имя"
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

        private static InputTextElement<AuthorizationModel, string, BotExampleContext> FirstNameTextInput = new()
        {
            Step = 0,
            NotifyMessage = "Write your first name",
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

        private static SelectMultipleElements<AuthorizationModel, InterestType, BotExampleContext> SelectIntersetsStep = new()
        {
            Step = 2,
            ItemsSupplier = (context) => Enum.GetValues<InterestType>().AsQueryable(),
            Pagination = new NextPrevPagination()
            {
                MaxElementsCount = 8,
            },

            PropertyName = nameof(AuthorizationModel.Interests),
            CallbackConvereter = (context) => Enum.Parse<InterestType>(context.Update.TrimCallbackCommand($"interest/")),

            CallbackDataTemplate = (context, interest) => 
            {
                //if()
                return $"check_interest/{interest}";
            },
            SelectedInlineButtonTextTemplate = (interest) => interest.ToString() + "✅",
            InlineButtonTextTemplate = (interest) => interest.ToString(),
            NotifyMessage = "Choose your interests!",
        };

        public static ILinkedStateMachine<BotExampleContext> CreateAuthPipeline(this ILinkedStateMachine<BotExampleContext> pipe)
        {
            pipe.Step(FirstNameTextInput, executionSequence: FirstNameTextInput.GetExecuteSequence);
            pipe.Step(SelectIntersetsStep, executionSequence: SelectIntersetsStep.GetExecuteSequence);

            return pipe;

            //Условие, шаг может быть обработан только единожды.
            //formHandler.ConfiramtionInfo = new ConfirmStepInfo()
            //{
            //    ConfirmationText = "Waiting for confirmation.",
            //    Step = 4,
            //    ResponsiblesToConfirmation = new()
            //    {
            //        new ResponsibleConfirmation()
            //        {
            //            Message = "Confirm authorization data.",
            //            Role = Role.Admin,
            //            InlineKeyboardMarkup = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
            //            {
            //                new InlineKeyboardButton("Confirm") { CallbackData = "395040322"},
            //                new InlineKeyboardButton("Cancel") { CallbackData = "395040322"},
            //            })
            //        }
            //    }
            //};
        }

        public static async Task<bool> ReplyKeyboardHandler<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
                where TContext : IUpdateContext
        {
            if (context.Update.Type == UpdateType.Message)
            {
                if (context.Update.Message.Text == "↩️ Назад")
                {
                    await prev(context, cancellationToken);
                    return true;
                }
            }
            return false;
        }
    }
}
