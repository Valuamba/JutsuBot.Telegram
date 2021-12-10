using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.TgBotFramework.UpdatePipeline
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(HandlerType handler) 
            : base($"{handler} was not found")
        {
            HandlerCode = (int) handler;
        }

        public int HandlerCode { get; }
    }

    public enum HandlerType
    {
        CallbackHandler = 0,
        ReplyKeyboardHandler = 2,
        StepHandler = 4,
        UpdateHandler = 8
    }
}
