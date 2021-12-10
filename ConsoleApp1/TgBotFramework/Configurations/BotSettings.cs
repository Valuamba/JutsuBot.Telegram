using System.Collections.Generic;

namespace TgBotFramework
{
    public class BotSettings : IBotSettings
    {
        public string ApiToken { get; set; }
        public string WebhookDomain { get; set; }
        public string WebhookPath { get; set; }
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string NewbieStage { get; set; }
        public int? NewbieStep { get; set; }
        public string AdministratorStage { get; set; }
        public int? AdministratorStep { get; set; }
        public List<long> Administrators { get; set; }
    }
}