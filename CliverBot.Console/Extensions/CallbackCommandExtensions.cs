﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CliverBot.Console.Extensions
{
    public static class CallbackCommandExtensions
    {
        public static bool IsCallbackCommand(this Update update, string command) =>
            update.CallbackQuery.Data.Contains(
                command,
                StringComparison.Ordinal);

        public static string TrimCallbackCommand(this Update update, string pattern) =>
            update.CallbackQuery.Data.Replace(pattern, string.Empty);
    }
}
