using CliverBot.Console.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess
{
    public class FormModel
    {
        public int FormId { get; set; }

        public long ChatId { get; set; }

        public string FormName { get; set; }

        public string FormCacheModel { get; set; }

        public TrackedMessage FormInformationMessage { get; set; }

        public List<TrackedMessage> FormUtilityMessages { get; set; }

        //public List<FormPropertyMetadata> FormProperties { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}
