using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace Htlv.Parser.DataAccess.EF.Entities
{
    public class User
    {
        public long Id { get; set; }
        public Role Role { get; set; }
        public string LanguageCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsBotStopped { get; set; }
        public State CurrentState { get; set; }
        public State PrevState { get; set; }
        public List<State> MessageState { get; set; }
    }
}
