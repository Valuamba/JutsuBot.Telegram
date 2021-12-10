using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.TgBotFramework.UpdatePipeline
{
    public class StepNotFoundException : Exception
    {
        public StepNotFoundException(string stage, long stateId)
            : base($"Step was not found. User with stage '{stage}' and state '{stateId}'.")
        {
        }
    }
}
