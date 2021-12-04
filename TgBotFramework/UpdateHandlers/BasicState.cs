using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework.Attributes;

namespace TgBotFramework
{
    public abstract class BasicState<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext 
    {
        public virtual async Task Enter(TContext state)
        {
            
        }
        
        public virtual async Task Exit(TContext state)
        {
            //state.UserState.Stage = "default";
            //state.UserState.Step = 0;
        }

        //protected Task RedirectToStep(TContext context, CancellationToken? cancellationToken = null, UpdateDelegate<TContext> next = null)
        //{
        //    context.UserState.Step +=2;
        //    return HandleAsync(context, next, cancellationToken ?? CancellationToken.None);
        //}

        //protected Task RedirectToStage<T>(TContext context, CancellationToken? cancellationToken = null, UpdateDelegate<TContext> next = null)
        //  where T : IUpdateHandler<TContext>
        //{
        //    var stateAttribute = typeof(T).GetCustomAttribute<StateAttribute>();

        //    context.UserState.Stage = stateAttribute.Stage;
        //    context.UserState.Step = 0;

        //    return ((T)context.Services.GetService(typeof(T))).HandleAsync(context, null, next, cancellationToken ?? CancellationToken.None);
        //}

        public abstract Task HandleAsync(TContext context, UpdateDelegate<TContext> next,
            CancellationToken cancellationToken);

        public Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}