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
        public List<State> States { get; set; } = new();

        public IEnumerable<State> GetStates(int userId, int messageId, StatePriority statePriority)
        {
            return States.Where(s => s.UserId == userId && s.MessageId == messageId || s.StatePriority == statePriority);
        }

        public State GetStateByMessageId(long userId, int messageId)
        {
            return States.Single(s => s.UserId == userId && s.MessageId == messageId);
        }

        public State GetStateByPriority(StatePriority statePriority, long userId)
        {
            return States.SingleOrDefault(s => s.UserId == userId && s.StatePriority == statePriority);
        }

        public State AddState(State state)
        {
            States.Add(state);
            return state;
        }
    }
}
