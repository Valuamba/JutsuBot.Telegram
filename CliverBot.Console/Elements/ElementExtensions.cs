using CliverBot.Console.DataAccess;
using CliverBot.Console.Extensions;
using CliverBot.Console.Form.Authorization;
using CliverBot.Console.Form.Elements;
using Htlv.Parser.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace CliverBot.Console.Form.v3.Elements
{
    public static class ElementExtensions
    {
        public static void Check<TContext>(this ILinkedStateMachine<TContext> pipe) where TContext : IUpdateContext
        {
            //SelectElement<AuthorizationModel, User, TContext> selectElement = new();

            //selectElement.ItemsSupplier = (context) =>
            //{
            //    var memoryRepository = (MemoryRepository)context.Services.GetService(typeof(MemoryRepository));
            //    return memoryRepository.GetUsersByRole(Role.Admin).AsQueryable();
            //};

            //selectElement.Pagination = new NextPrevPagination()
            //{
            //    MaxElementsCount = 8,
            //};

            //selectElement.PropertyName = nameof(AuthorizationModel.UserId);
            //selectElement.CallbackConvereter = (context) => Convert.ToInt64(context.Update.TrimCallbackCommand("userId/"));

            //selectElement.CallbackDataTemplate = (ctx) => $"userId/{ctx.Id}";
            //selectElement.InlineButtonTextTemplate = (ctx) => ctx.FullName;
            //selectElement.NotifyMessage = "Hola!";

            //pipe.Step(selectElement);
        }
    }
}
