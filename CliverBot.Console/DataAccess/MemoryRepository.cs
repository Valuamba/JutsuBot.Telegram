using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console.DataAccess
{
    public class MemoryRepository
    {
        public User UserState { get; set; }

        //public List

        public List<User> Users = new List<User>()
        {
            new User()
            {
                Id = 395040322,
                Role = Role.Visitor,
                FullName = "Valuamba",
                CurrentState = new State()
                {
                    Step = 0,
                    Stage = "Authorization"
                }
            },
            new User()
            {
                Id = 1607078784,
                Role = Role.Admin,
                FullName = "Sergei",
                CurrentState = new State()
                {
                    Step = 0,
                    Stage = "confirmAuthorization"
                }
            }
        };

        public User GetUserById(long userId)
        {
            return Users.Single(u => u.Id == userId);
        }

        public IList<User> GetUsersByRole(Role role)
        {
            return Users.Where(u => u.Role == role).ToList();
        }
    }
}
