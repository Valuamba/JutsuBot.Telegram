using CliverBot.Console.DataAccess.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Xunit;

namespace Jutsu.Telegram.Bot.Tests
{
    public class FormTests
    {
        [Fact]
        public void Should_SaveNotifyFormMessage()
        {
            //arrange

            Mock<FormRepository> formRepMock = new();

            //act
           

            //assert
        }
    }
}
