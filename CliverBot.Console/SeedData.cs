using CliverBot.Console.DataAccess.Entities;
using CliverBot.Console.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console
{
    public class SeedData
    {
        public void AddMessages(IServiceProvider serviceProvider)
        {
            var messageLocalizationRepository = (MessageLocalizationRepository) serviceProvider.GetService(typeof(MessageLocalizationRepository));

            messageLocalizationRepository.AddMessageLocalization("form.add", "ru", "Добавить");
            messageLocalizationRepository.AddMessageLocalization("form.change", "ru", "Изменить");

            messageLocalizationRepository.AddMessageLocalization("authorization.form", "ru", "Форма авторизации");
            messageLocalizationRepository.AddMessageLocalization("authorization.form.firstName", "ru", "Имя");
            messageLocalizationRepository.AddMessageLocalization("authorization.form.firstName.placeholder", "ru", "✍️...");
            messageLocalizationRepository.AddMessageLocalization("authorization.form.firstName.validationError.long", "ru", "Слишком длинное Имя.");

            messageLocalizationRepository.AddMessageLocalization("authorization.form.firstName.help.add", "ru", "Введите ваше <b>Имя</b>:");

        }
    }
}
