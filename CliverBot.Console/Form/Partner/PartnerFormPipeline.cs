using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Form.Partner
{
    public class PartnerFormPipeline
    {
        public static void CreatePartmerPipeline<TContext>(IFormHandlerBuilder<TContext> formHandler)
            where TContext : IUpdateContext
        {
            formHandler.Stage = "addPartner";
            formHandler.FormFields = new List<FormStepInfo<TContext>>()
            {
                new FormStepInfo<TContext>()
                {
                    Step = 0,
                    PropertyName = nameof(PartnerModel.BusinessName),
                    InformationText = "Write your partner business name.",
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
                    EntryReplyKeyboardMarkup = GetUndoKeyboardMarkup,
                    ReplyKeyboardHandler = HandleUndoButton
                },
                new FormStepInfo<TContext>()
                {
                    Step = 2,
                    PropertyName = nameof(PartnerModel.Town),
                    InformationText = "Write partner town.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "There is available 1000 symbols.",
                            UpdatePredicate = (context) => context.Update.Message?.Text?.Length < 1000
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 4,
                    PropertyName = nameof(PartnerModel.Address),
                    InformationText = "Write partner address.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "There is available 1000 symbols.",
                            UpdatePredicate = (context) => context.Update.Message?.Text?.Length < 1000
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 6,
                    PropertyName = nameof(PartnerModel.Mall),
                    InformationText = "Write partner mall.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "There is available 1000 symbols.",
                            UpdatePredicate = (context) => context.Update.Message?.Text?.Length < 1000
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackandSkiptKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 8,
                    PropertyName = nameof(PartnerModel.CountOfUnitsOnAutumnAndWinter),
                    InformationText = "Write partner winter and autumn.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Некорректные входные данные.\r\n" +
                                "Проверьте, что введенное вами число не содержит букв и других символов, а также, не превышает длину в 9 символов.",
                            UpdatePredicate = (context) => new Regex("^\\d{0,9}$").IsMatch(context.Update.Message?.Text)
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackandSkiptKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 10,
                    PropertyName = nameof(PartnerModel.CountOfUnitsOnSpring),
                    InformationText = "Write partner spring.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Некорректные входные данные.\r\n" +
                                "Проверьте, что введенное вами число не содержит букв и других символов, а также, не превышает длину в 9 символов.",
                            UpdatePredicate = (context) => new Regex("^\\d{0,9}$").IsMatch(context.Update.Message?.Text)
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackandSkiptKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 12,
                    PropertyName = nameof(PartnerModel.CountOfUnitsOnSummer),
                    InformationText = "Write partner summer.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Некорректные входные данные.\r\n" +
                                "Проверьте, что введенное вами число не содержит букв и других символов, а также, не превышает длину в 9 символов.",
                            UpdatePredicate = (context) => new Regex("^\\d{0,9}$").IsMatch(context.Update.Message?.Text)
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackandSkiptKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 14,
                    PropertyName = nameof(PartnerModel.EMail),
                    InformationText = "Write partner email.",
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
                new FormStepInfo<TContext>()
                {
                    Step = 16,
                    PropertyName = nameof(PartnerModel.MobilePhone),
                    InformationText = "Write partner phone.",
                    ValidationHandlers = new List<ValidationHandler<TContext>>()
                    {
                        new ValidationHandler<TContext>()
                        {
                            ErrorMessage = "Некорректные входные данные.\r\n" +
                                "Проверьте, что введенное вами число не содержит букв и других символов, а также, не превышает длину в 9 символов.",
                            UpdatePredicate = (context) => new Regex("^\\d{0,9}$").IsMatch(context.Update.Message?.Text)
                        },
                    },
                    EntryReplyKeyboardMarkup = GetSetBackandSkiptKeyboardMarkup,
                    ReplyKeyboardHandler = HandleSetBackAndSkipButtons
                },
            };

            formHandler.ExtendedPrevDelegate = (node) => async(context, cancellationToken) =>
            {
                context.Update.ClearUpdate();
                context.UserState.CurrentState.Step -= 3;
                await node.Previous?.Data(context, cancellationToken);
            };

            formHandler.ExtendedNextDelegate = (node) => async (context, cancellationToken) =>
            {
                context.Update.ClearUpdate();
                context.UserState.CurrentState.Step++;
                await node.Next?.Data(context, cancellationToken);
            };

            //Условие, шаг может быть обработан только единожды.
            formHandler.ConfiramtionInfo = new ConfirmStepInfo()
            {
                ConfirmationText = "Waiting for confirmation.",
                Step = 4,
            };
        }

        public static ReplyKeyboardMarkup GetUndoKeyboardMarkup => new ReplyKeyboardMarkup(
            new List<IEnumerable<KeyboardButton>>
            {
                new List<KeyboardButton> { "❌ Отмена" },
            })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup GetSetBackKeyboardMarkup => new ReplyKeyboardMarkup(
            new List<IEnumerable<KeyboardButton>>
            {
                new List<KeyboardButton> { "↩️ Назад" },
                new List<KeyboardButton> { "❌ Отмена" },
            })
            {
                ResizeKeyboard = true
            }; 

        public static ReplyKeyboardMarkup GetSetBackandSkiptKeyboardMarkup => new ReplyKeyboardMarkup(
            new List<IEnumerable<KeyboardButton>>
            {
                new List<KeyboardButton> { "↩️ Назад" },
                new List<KeyboardButton> { "⤵️ Пропустить" },
                new List<KeyboardButton> { "❌ Отмена" },
            })
        {
            ResizeKeyboard = true
        };

        public static async Task<bool> HandleSetBackAndSkipButtons<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
                where TContext : IUpdateContext
        {
            if (context.Update.Type == UpdateType.Message)
            {
                switch (context.Update.Message.Text)
                {
                    case "↩️ Назад": await prev(context, cancellationToken); return true;

                    case "⤵️ Пропустить": await next(context); return true;

                    //case "❌ Отмена": await RedirectToStage<MenuStage>(context); break;

                    default: return false;
                }
            }
            return false;
        }

        public static async Task<bool> HandleUndoButton<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
               where TContext : IUpdateContext
        {
            if (context.Update.Type == UpdateType.Message)
            {
                switch (context.Update.Message.Text)
                {
                    //Как покидать стейдж и куда ухожить?
                    case "❌ Отмена": /*await RedirectToStage<PartnerMenuStage>(context);*/ break;

                    default: return false;
                }
            }
            return false;
        }

        public static async Task<bool> HandleSetBackButtons<TContext>(UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, TContext context, CancellationToken cancellationToken)
                where TContext : IUpdateContext
        {
            if (context.Update.Type == UpdateType.Message)
            {
                switch (context.Update.Message.Text)
                {
                    case "↩️ Назад": await prev(context, cancellationToken); return true;

                    //Как покидать стейдж и куда ухожить?
                    case "❌ Отмена": /*await RedirectToStage<PartnerMenuStage>(context);*/ break;

                    default: return false;
                }
            }
            return false;
        }
    }
}
