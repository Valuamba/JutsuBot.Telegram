using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Handlers.Authorization.Handlers
{
    public class IndexHandler : AuthorizationNameHandler
    {
        public IndexHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }
    }
}
