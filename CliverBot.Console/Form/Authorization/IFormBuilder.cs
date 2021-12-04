using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.Form.Authorization
{
    public interface IFormBuilder<TContext> where TContext : IUpdateContext
    {
        public ILinkedStateMachine<TContext> BuildForm();
    }
}
