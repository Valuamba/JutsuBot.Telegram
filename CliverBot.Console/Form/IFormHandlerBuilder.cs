using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.Form
{
    public interface IFormHandlerBuilder<TContext> where TContext : IUpdateContext
    {
        public List<FormStepInfo<TContext>> FormFields { get; set; }
        public string Stage { get; set; }
        public ConfirmStepInfo ConfiramtionInfo { get; set; }
        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> ExtendedPrevDelegate { get; set; }
        public Func<LinkedNode<TContext>, UpdateDelegate<TContext>> ExtendedNextDelegate { get; set; }
    }
}
