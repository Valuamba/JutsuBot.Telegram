using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CliverBot.Console.Pagination
{
    public static class Row
    {
        private const int maxCount = 3;

        public static IEnumerable<InlineKeyboardButton> Pagination(int count, int page = 1)
        {
            List<InlineKeyboardButton> controls = new(3);

            if (page == 1 && count <= maxCount)
            {
                return null;
            }
            else
            {
                if (page == 1)
                {
                    controls.Add(InlineKeyboardButton.WithCallbackData(">", $"{Constants.ChangeTo}{page + 1}"));
                }
                else
                {
                    controls.Add(InlineKeyboardButton.WithCallbackData("<", $"{Constants.ChangeTo}{page - 1}"));

                    var c = count / (double)(page * maxCount);

                    if (c > 1)
                    {
                        controls.Add(InlineKeyboardButton.WithCallbackData(">", $"{Constants.ChangeTo}{page + 1}"));
                    }
                }
            }

            return controls;
        }

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> BuildButtonsList<T>(
            IEnumerable<T> data, 
            Func<T, string> textButtonSelector, 
            Func<T, string> callbackButtonSelector, 
            int page = 1)
        {
            var usedData = data
                  .Skip((page - 1) * maxCount)
                  .Take(maxCount)
                  .ToList();

            List<List<InlineKeyboardButton>> buttons = new();
            for (int i = 0; i < usedData.Count(); i++)
            {
                buttons.Add(
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData(
                            textButtonSelector(data.ElementAt(i)),
                            callbackButtonSelector(data.ElementAt(i)))
                    });
            }

            return buttons;
        }


        //public static IEnumerable<IEnumerable<InlineKeyboardButton>> ActionWithUser(long userId, int prevPage)
        //{
        //    return new List<List<InlineKeyboardButton>>()
        //    {
        //        new () { InlineKeyboardButton.WithCallbackData("Убрать должность", Constants.RemovePosition + userId)},
        //        new () { InlineKeyboardButton.WithCallbackData("Назад", Constants.Back + prevPage) },
        //    };
        //}

        public static IEnumerable<InlineKeyboardButton> Controls(int startIndex, int prevIndex, int pageNumber) =>
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "<",
                    $"{string.Format(Constants.ChangeTo, startIndex, prevIndex, pageNumber)}"
                ),
                " ",
                InlineKeyboardButton.WithCallbackData(
                    ">",
                    $"{string.Format(Constants.ChangeTo, startIndex, prevIndex, pageNumber)}"
                ),
            };

        public static IEnumerable<InlineKeyboardButton> BackToMenu() =>
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Назад", Constants.BackToMenu)
            };
    }
}
