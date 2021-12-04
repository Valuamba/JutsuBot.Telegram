using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console.Elements.InputTextElement
{
    public interface IInputTextElementContext : IUpdateContext
    {
        public IInputTextClient<IInputTextElementContext> InputTextClient { get; set; }
    }
}
