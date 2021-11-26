using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.DataAccess
{
    public class MemoryRepositoryMiddleware : ICallbackButtonHandler<BotExampleContext>
    {
        private readonly MemoryRepository _memoryRepository;

        public MemoryRepositoryMiddleware(MemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var user = _memoryRepository.GetUserById(context.Update.GetSenderId());
            if (user == null)
            {
                user= new User()
                {
                    CurrentState = new()
                    {
                        Stage = "Authorization",
                        Step = 0
                    },
                   Id = context.Update.GetSenderId(),
                   Role = Role.Visitor,
                   IsBotStopped = false,
                };
            }

            context.UserState = MapUserToUserState(user);

            await next(context);

            _memoryRepository.UserState = MapUserStateToUser(context.UserState, user);
        }

        private static UserState MapUserToUserState(User user)
            => new()
            {
                CurrentState = user.CurrentState,
                IsBotStopped = user.IsBotStopped,
                FullName = user.FullName,
                IsAuthorized = user.IsAuthorized,
                LanguageCode = user.LanguageCode,
                PhoneNumber = user.PhoneNumber,
                PrevState = user.PrevState,
                Role = user.Role
            };

        private static User MapUserStateToUser(IUserState userState, User user)
        {
            user.CurrentState = userState.CurrentState;
            user.IsBotStopped = userState.IsBotStopped;
            user.FullName = userState.FullName;
            user.IsAuthorized = userState.IsAuthorized;
            user.LanguageCode = userState.LanguageCode;
            user.PhoneNumber = userState.PhoneNumber;
            user.PrevState = userState.PrevState;
            user.Role = userState.Role;

            return user;
        }
    }
}
