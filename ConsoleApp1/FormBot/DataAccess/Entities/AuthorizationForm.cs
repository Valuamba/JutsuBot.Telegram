using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess.Entities
{
    public class AuthorizationForm
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDate { get; set; }
       // public List<InterestType> Interests { get; set; }
        public int Age { get; set; }
        public bool IsSubmitted { get; set; }

        public FormModel Form { get; set; }
    }
}
