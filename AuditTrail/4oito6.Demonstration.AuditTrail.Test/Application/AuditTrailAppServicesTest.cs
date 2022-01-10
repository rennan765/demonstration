using _4oito6.Demonstration.AuditTrail.Receiver.Application;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using System;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.AuditTrail.Test.Application
{
    [Trait("AuditTrailAppServices", "Application tests")]
    public class AuditTrailAppServicesTest
    {
        [Fact(DisplayName = "Testing if message is null.")]
        public async Task ProcessMessageAsync_ArgumentNullException()
        {
            //arrange
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<AuditTrailAppServices>
            (
                mocker.GetMock<IAuditTrailServices>().Object,
                mocker.GetMock<ILogger<AuditTrailAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            AuditTrailMessage message = null;
            var ex = new ArgumentNullException(nameof(message));

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<ArgumentNullException>(), null))
                .Verifiable();

            //act:
            await appServiceMock.Object.ProcessMessageAsync(message).ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Message should process successfully.")]
        public async Task ProcessMessageAsync_Success()
        {
            //arrange
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<AuditTrailAppServices>();

            var message = new AuditTrailMessage
            (
                id: Guid.NewGuid(),
                code: "TESTE_IN01",
                message: string.Empty,
                date: DateTime.UtcNow.Date
            );

            mocker.GetMock<IAuditTrailServices>()
                .Setup(s => s.ProcessAsync(It.Is<AuditTrailMessage>(m => m.Match(message, false))))
                .Verifiable();

            //act:
            await appService.ProcessMessageAsync(message).ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }

        [Fact(DisplayName = "Error while trying to process successfully.")]
        public async Task ProcessMessageAsync_Exception()
        {
            //arrange
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<AuditTrailAppServices>
            (
                mocker.GetMock<IAuditTrailServices>().Object,
                mocker.GetMock<ILogger<AuditTrailAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var message = new AuditTrailMessage
            (
                id: Guid.NewGuid(),
                code: "TESTE_IN01",
                message: string.Empty,
                date: DateTime.UtcNow.Date
            );

            var ex = new InvalidOperationException();
            mocker.GetMock<IAuditTrailServices>()
                .Setup(s => s.ProcessAsync(It.Is<AuditTrailMessage>(m => m.Match(message, false))))
                .ThrowsAsync(ex)
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<InvalidOperationException>(), null))
                .Verifiable();

            //act:
            await appServiceMock.Object.ProcessMessageAsync(message).ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Message should be sent successfully.")]
        public async Task SendAsync_Success()
        {
            //arrange
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<AuditTrailAppServices>();

            var code = "TESTE_IN01";
            var message = "erro";
            var additionalInformation = "teste unitário";

            mocker.GetMock<IAuditTrailSender>()
                .Setup(s => s.SendAsync(code, message, additionalInformation))
                .Verifiable();

            //act:
            await appService.SendAsync(code, message, additionalInformation).ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }

        [Fact(DisplayName = "Error while trying to send message.")]
        public async Task SendAsync_Exception()
        {
            //arrange
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<AuditTrailAppServices>
            (
                mocker.GetMock<IAuditTrailServices>().Object,
                mocker.GetMock<ILogger<AuditTrailAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var code = "TESTE_IN01";
            var message = "erro";
            var additionalInformation = "teste unitário";

            appServiceMock
                .SetupGet(s => s.AuditTrail)
                .Returns(mocker.GetMock<IAuditTrailSender>().Object)
                .Verifiable();

            var ex = new InvalidOperationException();
            mocker.GetMock<IAuditTrailSender>()
                .Setup(s => s.SendAsync(code, message, additionalInformation))
                .ThrowsAsync(ex)
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<InvalidOperationException>(), null))
                .Verifiable();

            //act:
            await appServiceMock.Object.SendAsync(code, message, additionalInformation).ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }
    }
}