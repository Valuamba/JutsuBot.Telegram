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

namespace CliverBot.Console.Form.Authorization
{
    public static class AuthFormPipeline
    {
        public static void CreateAuthPipeline<TContext>(IFormHandlerBuilder<TContext> formHandler)
            where TContext : IUpdateContext
        {
            formHandler.Stage = "Authorization";
            formHandler.FormFields = new List<FormStepInfo<TContext>>()
            {
                new FormStepInfo<TContext>()
                {
                    Step = 0,
                    PropertyName = nameof(AuthorizationModel.Email),
                    InformationText = "Write your email.",
                    ErrorText = "Incorrect email.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Incorrect format.",
                            UpdatePredicate = (context) => context.Update.Message?.Text.Contains("@") ?? false
                        },
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Word is too long.",
                            UpdatePredicate = (context) => context.Update.Message?.Text?.Length < 160
                        },
                    },
                    ReplyKeyboardHandler = ReplyKeyboardHandler
                },
                new FormStepInfo<TContext>()
                {
                    Step = 2,
                    PropertyName = nameof(AuthorizationModel.FirstName),
                    InformationText = "Write your first name.",
                    ErrorText = "Incorrect first name.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        //new ValidationHandler<TContext>()
                        //{
                        //    ErrorMessage = "Incorrect format.",
                        //    UpdatePredicate = (context) => new Regex(@"^[А-Яа-я]+[\s][А-Яа-я]+\s[А-Яа-я]+$").IsMatch(context.Update.Message?.Text)
                        //},
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Word is too long.",
                            UpdatePredicate = (context) => context.Update.Message?.Text?.Length < 160
                        },
                    },
                    ReplyKeyboardHandler = ReplyKeyboardHandler
                },
            };

            //Условие, шаг может быть обработан только единожды.
            formHandler.ConfiramtionInfo = new ConfirmStepInfo()
            {
                ConfirmationText = "Waiting for confirmation.",
                Step = 4,
                ResponsiblesToConfirmation = new()
                {
                    new ResponsibleConfirmation()
                    {
                        Message = "Confirm authorization data.",
                        Role = Role.Admin,
                        InlineKeyboardMarkup = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
                        {
                            new InlineKeyboardButton("Confirm") { CallbackData = "Confirm"},
                            new InlineKeyboardButton("Cancel") { CallbackData = "Cancel"},
                        })
                    }
                }
            };
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
