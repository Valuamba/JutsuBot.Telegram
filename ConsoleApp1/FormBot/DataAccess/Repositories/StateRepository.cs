using CliverBot.Console.DataAccess;
using Jutsu.Telegarm.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JutsuBot.Elements.DataAccess.Repositories
{
    public class StateRepository
    {
        private readonly ApplicationDbContext _context;

        public StateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<State> GetStates(int userId, int messageId, StatePriority statePriority)
        {
            return _context.States.Where(s => s.UserId == userId && s.MessageId == messageId || s.StatePriority == statePriority);
        }

        public State GetStateByMessageId(long userId, int messageId)
        {
            return _context.States.Single(s => s.UserId == userId && s.MessageId == messageId);
        }

        public State GetStateByPriority(StatePriority statePriority, long userId)
        {
            return _context.States.SingleOrDefault(s => s.UserId == userId && s.StatePriority == statePriority);
        }

        public State AddState(State state)
        {
            _context.States.Add(state);
            _context.SaveChanges();
            return state;
        }
    }
}
