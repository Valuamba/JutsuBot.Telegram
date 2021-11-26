using CliverBot.Console.DataAccess;
using CliverBot.Console.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Pagination.Users
{
    public class ManageUsersStep
    {
        private readonly MemoryRepository _memoryRepository;

        public ManageUsersStep(MemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        public void Build()
        {
            ListComponentInfo<User, BaseUpdateContext> users = new();

            users.AddRange(_memoryRepository.Users);

            //users.ComponentCallbackHandler = 
        }
    }
}
