using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Handlers.Authorization.Handlers
{
    public class MMRHandler : AuthorizationNameHandler
    {
        public MMRHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }
    }
}
