using CliverBot.Console.DataAccess;
using JutsuBot.Elements.DataAccess;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace Htlv.Parser.DataAccess.EF
{
    public class UserStateMapperMiddleware<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly ApplicationDbContext _context;

        public UserStateMapperMiddleware(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var user = context.Update.GetSender();

            User userDbObject = null;
            if (user.Id != 0)
            {
                userDbObject = _context.Users.SingleOrDefault(u => u.Id == context.Update.GetSenderId());
                if (userDbObject == null)
                {
                    userDbObject = new User();
                    userDbObject.Id = user.Id;
                    userDbObject.FullName = user.Username ?? $"{user.FirstName} {user.LastName}";
                    userDbObject.Role = Role.Visitor;
                    //userDbObject.CurrentState = new CliverBot.Console.DataAccess.State()
                    //{
                    //    Step = 0,
                    //    Stage = "default",
                    //    UserForCurrentState = userDbObject
                    //};

                    await _context.Users.AddAsync(userDbObject);
                    await _context.SaveChangesAsync();
                }
                context.UserState ??= new UserState();
                //context.UserState.CurrentState ??= new ();
                //context.UserState.PrevState ??= new();

                UserModelMapper.MapModelToState(context.UserState, userDbObject);
            }

            await next(context, cancellationToken);

            if(UserModelMapper.MapStateToModel(context.UserState, userDbObject))
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
