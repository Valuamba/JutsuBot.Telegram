using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console.Form
{
    public class FormHandlerBuilder<TContext> : IFormHandlerBuilder<TContext> where TContext : IUpdateContext
    {
        public List<FormStepInfo<TContext>> FormFields { get; set; }
        public ConfirmStepInfo ConfiramtionInfo { get; set; }
        public string Stage { get; set; }

    }
}
