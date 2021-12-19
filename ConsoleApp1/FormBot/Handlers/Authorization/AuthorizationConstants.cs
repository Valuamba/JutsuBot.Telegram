using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    public class AuthorizationConstants
    {
        public const string CHANGE_FIELD_CALLBACK_PATTERN = "change?name={0}&userId={1}&formId={2}";

        public const string CHOOSE_INTEREST_TYPE_CALLBACK_PATTERN = "choose_interest?interest={0}&formId={1}";

        public const string CHANGE_FORM_INPUT = "prevStep";
    }
}
