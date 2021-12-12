using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRole = TgBotFramework.Role;

namespace JutsuForms.Server.FormBot.Predicates
{
    public class IsRole
    {
        public static bool Admin(BotExampleContext context) => context.UserState.Role == UserRole.Admin;
        public static bool Visitor(BotExampleContext context) => context.UserState.Role == UserRole.Visitor;
    }
}
