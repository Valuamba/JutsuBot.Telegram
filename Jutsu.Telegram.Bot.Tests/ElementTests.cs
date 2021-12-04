using CliverBot.Console.Form.v3.Elements;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using Xunit;

namespace Jutsu.Telegram.Bot.Tests
{
    public class ElementTests
    {
        [Fact]
        public async Task Should_SaveMessage_WithStatus_ToDelete()
        {
            //arrange
            BaseUpdateContext context = new();

            IMessageRepository messageRepository = new();
            context.Services.
            InputTextElement<TestModel, string, BaseUpdateContext> inputTextElement = new()
            {
                NotifyMessage = "Write .....",
                PropertyName = nameof(TestModel.TestProperty),
                Step = 0,
            };

            //act
            await inputTextElement.NotifyStep(context, CancellationToken.None);

            //assert
            messageRepository.Verify(m => m.AddMessage(It.IsAny<int>(), MessageStatus.ReadyToDelete));
        }

        [Fact]
        public async Task Should_DeleteFormMethod()
        {
            //arrange
            BaseUpdateContext context = new();

            int userId = 12;

            IMessageRepository messageRepository = new();
            InputTextElement<TestModel, string, BaseUpdateContext> inputTextElement = new()
            {
                NotifyMessage = "Write .....",
                PropertyName = nameof(TestModel.TestProperty),
                Step = 0,
            };

            //act
            await inputTextElement.HandleAsync(context, null, null, CancellationToken.None);

            //assert
            messageRepository.Verify(m => m.DeleteMessages(It.IsAny<int>(), userId, MessageStatus.ReadyToDelete));
        }
    }

    public class TestModel
    {
        public string TestProperty { get; set; }
    }
}
