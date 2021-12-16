using ConsoleApp1.FormBot.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Jutsu.Telegram.Bot.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string cache = null;

            cache = cache.AddItemToArray(123, "list");
            cache = cache.AddItemToArray(321, "list");
            cache = cache.AddItemToArray(421, "list");
            cache = cache.AddItemToArray(421, "list");

            cache = cache.RemoveProperty("list");


            cache = cache.AddProperty("Petr", nameof(AuthorizationModel.Name));
            cache = cache.AddProperty("Petr_222", nameof(AuthorizationModel.Name));
            cache = cache.RemoveProperty(nameof(AuthorizationModel.Name));
            cache = cache.RemoveProperty(nameof(AuthorizationModel.Name));
        }
    }

    public class AuthorizationModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Interests { get; set; }
    }
}
