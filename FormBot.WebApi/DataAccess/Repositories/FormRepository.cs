using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess.Repositories
{
    public class FormRepository
    {
        public List<FormModel> Forms { get; set; }

        public FormModel AddForm(FormModel form)
        {
            Forms.Add(form);
            return form;
        }

        public FormModel GetFormById(int formId)
        {
            return Forms.Single(f => f.FormId == formId);
        }
    }
}
