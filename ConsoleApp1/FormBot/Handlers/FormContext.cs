using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Handlers
{
    public class FormContext
    {
        private List<FormHandlerContext> _formHandlersContext = new List<FormHandlerContext>();

        public string FormName = "Authorization Form";

        public void AddFormHandlerContext(FormHandlerContext formHandlerContext)
        {
            _formHandlersContext.Add(formHandlerContext);
        }

        public List<FormHandlerContext> FormHandlersContext
            => _formHandlersContext;
    }

    public class FormHandlerContext
    {
        public int Step { get; set; }
        public string Stage { get; set; }
        public string FieldName { get; set; }
    }
}
