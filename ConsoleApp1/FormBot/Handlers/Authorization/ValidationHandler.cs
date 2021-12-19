using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBotFramework;

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    public class ValidationHandler<TContext> where TContext : IUpdateContext
    {
        public Predicate<TContext> UpdatePredicate { get; set; }

        public string ErrorMessageAlias { get; set; }
    }
}
