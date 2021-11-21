using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console.Form
{
    public class ValidationHandler<TContext> where TContext : IUpdateContext
    {
        public Predicate<TContext> UpdatePredicate { get; set; }
        public string ErrorMessage { get; set; }
    }
}
