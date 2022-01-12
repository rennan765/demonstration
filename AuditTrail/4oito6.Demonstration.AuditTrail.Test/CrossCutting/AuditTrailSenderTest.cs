using _4oito6.Demonstration.CrossCutting.AuditTrail;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.AuditTrail.Test.CrossCutting
{
    [Trait("AuditTrailSender", "CrossCutting tests")]
    public class AuditTrailSenderTest
    {
        private readonly CompareLogic _comparison = new();
        private readonly string _queue = "http://sqs.com";

        [Fact(DisplayName = "Message successfully sended.")]
        public async Task SendAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var sender = new AuditTrailSender(mocker.GetMock<IAmazonSQS>().Object, _queue);

            var message = new AuditTrailMessage
            (
                id: Guid.NewGuid(),
                code: "TESTE_EX01",
                message: "erro",

                date: DateTime.UtcNow.Date,
                additionalInformation: "teste unitário"
            );

            var request = new SendMessageRequest
            {
                DelaySeconds = 5,
                MessageBody = JsonConvert.SerializeObject(message),
                QueueUrl = _queue
            };

            mocker.GetMock<IAmazonSQS>()
                .Setup(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => _comparison.Compare(request, r).AreEqual), default))
                .Verifiable();

            //act:
            await sender.SendAsync(message).ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }

        [Fact(DisplayName = "Parameter method: message successfully sended.")]
        public async Task SendAsync_Parameters_Success()
        {
            //arrange:
            var senderMock = new Mock<AuditTrailSender>();

            var message = new AuditTrailMessage
            (
                code: "TESTE_EX01",
                message: "erro",
                additionalInformation: "teste unitário"
            );

            senderMock
                .Setup(s => s.SendAsync(It.Is<AuditTrailMessage>(m => m.Match(message, false))))
                .Verifiable();

            //act:
            await senderMock.Object
                .SendAsync(message.Code, message.Message, message.AdditionalInformation)
                .ConfigureAwait(false);

            //assert:
            Mock.Verify(senderMock);
        }
    }
}