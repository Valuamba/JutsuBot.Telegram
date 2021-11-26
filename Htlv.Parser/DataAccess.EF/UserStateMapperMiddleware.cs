using Htlv.Parser.DataAccess.EF.Entities;
using Microsoft.EntityFrameworkCore;
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
    public class UserStateMapperMiddleware<TContext> : ICallbackButtonHandler<TContext> where TContext : IUpdateContext
    {
        private readonly BotDbContext _db;
        private readonly IOptions<BotSettings> _botOptions;

        public UserStateMapperMiddleware(BotDbContext db)
        {
            _db = db;
            //_botOptions = botOptions;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var user = context.Update.GetSender();

            User userDbObject = null;
            if (user.Id != 0)
            {
                userDbObject = await _db.Users.Include(u => u.CurrentState).Include(u => u.PrevState).SingleOrDefaultAsync(u => u.Id == user.Id);
                if (userDbObject == null)
                {
                    userDbObject = new User();
                    userDbObject.Id = user.Id;
                    userDbObject.FullName = user.Username ?? $"{user.FirstName} {user.LastName}";
                    userDbObject.Role = Role.Visitor;
                    userDbObject.CurrentState = new Entities.State()
                    {
                        Step = 0,
                        Stage = "default",
                        UserForCurrentState = userDbObject
                    };

                    await _db.Users.AddAsync(userDbObject, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                    userDbObject = await _db.Users.FindAsync(user.Id);
                }
                context.UserState ??= new UserState();
                context.UserState.CurrentState ??= new ();
                context.UserState.PrevState ??= new();

                UserModelMapper.MapModelToState(context.UserState, userDbObject);
            }

            await next(context, cancellationToken);

            if (userDbObject != null && UserModelMapper.MapStateToModel(context.UserState, userDbObject))
            {
                _db.Users.Update(userDbObject);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
