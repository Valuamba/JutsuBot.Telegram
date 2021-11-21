using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace CliverBot.Console.Form
{
    public class ConfirmStepInfo
    {
        public string ConfirmationText { get; set; }
        public int Step { get; set; }
        public List<ResponsibleConfirmation> ResponsiblesToConfirmation { get; set; }
    }

    public class ResponsibleConfirmation
    {
        public Role? Role { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; }
        public InlineKeyboardMarkup InlineKeyboardMarkup { get; set; }
    }
}
