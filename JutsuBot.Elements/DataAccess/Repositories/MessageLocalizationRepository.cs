using CliverBot.Console.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess.Repositories
{
    public class MessageLocalizationRepository
    {
        public List<MessageLocalization> MessageLocalizations { get; set; }

        public void AddMessageLocalization(string alias, string language, string text)
        {
            MessageLocalizations.Add(new MessageLocalization() { Alias = alias, Language = language, Text = text });
        }

        public string GetMessage(string alias, string language = "ru")
        {
            return MessageLocalizations.Single(m => m.Alias == alias && m.Language == language).Text;
        }
    }
}
