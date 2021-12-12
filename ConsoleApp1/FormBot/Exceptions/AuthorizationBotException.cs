using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Exceptions
{
    public class AuthorizationBotException : Exception
    {
        public AuthorizationBotException(string message) : base(message)
        {

        }
    }
}
