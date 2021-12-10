using Jutsu.Telegarm.Bot.Models;

namespace TgBotFramework
{
    public class UserState : IUserState
    { 
        public Role Role { get; set; }
        public string LanguageCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsBotStopped { get; set; }
        public State CurrentState { get; set; }
        public State PrevState { get; set; }
    }

    public interface IUserState
    {
        public State CurrentState { get; set; }
        public State PrevState { get; set; }
        public Role Role { get; set; }
        public int Age { get; set; }
        public string LanguageCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsBotStopped { get; set; }
    }

    public class State
    {
        public string CacheData { get; set; }
        public string Stage { get; set; }
        public long? Step { get; set; }
        public StatePriority StatePriority { get; set; }

        public int? MessageId { get; set; }
    }
}