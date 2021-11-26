using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Xunit;

namespace Jutsu.Telegram.Bot.Tests
{
    public class LinkedStateMachineTests
    {
        public async Task Handlers(UpdateDelegate<BaseUpdateContext> prev, UpdateDelegate<BaseUpdateContext> next, BaseUpdateContext context, CancellationToken cancellationToken)
        {

        }

        [Fact]
        public void TestLinkedStateMachine()
        {
            //arrange
            Mock<HandlerDelegate<BaseUpdateContext>> handlerDelegateMock = new();
            BaseUpdateContext context = new() { UserState = new UserState() { CurrentState = new() { Step = 1 } } };

            //act
            var linkedStateMachine = new LinkedStateMachine<BaseUpdateContext>(new Microsoft.Extensions.DependencyInjection.ServiceCollection());
            linkedStateMachine.Step(null, null, handlerDelegateMock.Object, null);

            linkedStateMachine.Head.Data(context);

            //assert
            handlerDelegateMock.Verify(p => p(
                It.IsAny<UpdateDelegate<BaseUpdateContext>>(), 
                It.IsAny<UpdateDelegate<BaseUpdateContext>>(), 
                It.IsAny<BaseUpdateContext>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
