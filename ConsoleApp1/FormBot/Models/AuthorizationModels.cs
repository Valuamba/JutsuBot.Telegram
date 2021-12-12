using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.FormBot.Models
{
    public class AuthorizationModels
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<IntersetType> Interests { get; set; }

    }

    public enum IntersetType
    {
        Dota2,
        LoL,
        WoW,
    }
}
