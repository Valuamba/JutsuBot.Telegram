using System;
using Microsoft.Extensions.Logging;

namespace TgBotFramework.UpdatePipeline
{
    public interface IBotPipelineBuilder<TContext> where TContext : IUpdateContext
    {
        ILogger<IBotPipelineBuilder<TContext>> Logger { get; }
        
        IBotPipelineBuilder<TContext> Use(Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>> middleware);
        IBotPipelineBuilder<TContext> Use<THandler>() 
            where THandler : ICallbackButtonHandler<TContext>;
        IBotPipelineBuilder<TContext> Use<THandler>(THandler handler) 
            where THandler : ICallbackButtonHandler<TContext>;
        
        
        IBotPipelineBuilder<TContext> UseWhen<THandler>(Predicate<TContext> predicate)
            where THandler : ICallbackButtonHandler<TContext>;
        IBotPipelineBuilder<TContext> UseWhen(Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure);


        IBotPipelineBuilder<TContext> MapWhen(Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure);
        IBotPipelineBuilder<TContext> MapWhen<THandler>(Predicate<TContext> predicate)
            where THandler : ICallbackButtonHandler<TContext>;

        IBotPipelineBuilder<TContext> UseCommand<TCommand>(string command) 
            where TCommand : CommandBase<TContext>;

        IBotPipelineBuilder<TContext> UseUnorderedHandlerCommand<THandler>(
            Predicate<TContext> predicate)
            where THandler : ICallbackButtonHandler<TContext>;

        UpdateDelegate<TContext> Build();
    }
}