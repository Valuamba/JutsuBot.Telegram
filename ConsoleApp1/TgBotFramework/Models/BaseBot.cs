using Microsoft.Extensions.Options;

namespace TgBotFramework
{
    public class BaseBot 
    {
        public string Username { get; }

        public BaseBot()
        {
            //Username = options.Value.Username;
        }
    }
}