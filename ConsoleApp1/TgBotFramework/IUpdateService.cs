using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.TgBotFramework
{
    public interface IUpdateService
    {
        Task SendMessage(long userId, string message);
    }
}
