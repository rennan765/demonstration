using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using System;
using Xunit;

namespace _4oito6.Demonstration.AuditTrail.Test.CrossCutting.Model
{
    [Trait("AuditTrailMessage", "Domain Model tests")]
    public class AuditTrailMessageTest
    {
        [Fact(DisplayName = "Handling null while trying to set Exception.")]
        public void SetException_ArgumentNull()
        {
            new AuditTrailMessage().Invoking(at => at.SetException(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Exception should be setted successfully.")]
        public void SetException_Success()
        {
            //arrange:
            var exMock = new Mock<Exception>();
            var message = "domain service error";
            var stackTrace = "at 4oito6 blahblahblah";

            exMock.SetupGet(ex => ex.Message).Returns(message).Verifiable();
            exMock.SetupGet(ex => ex.StackTrace).Returns(stackTrace).Verifiable();

            var expectedResult = JsonConvert.SerializeObject(new
            {
                Message = message,
                StackTrace = stackTrace,
                InnerAdditionalInformation = (object)null
            });

            //act:
            var auditTrailMessage = new AuditTrailMessage();
            auditTrailMessage.SetException(exMock.Object);

            //assert:
            auditTrailMessage.AdditionalInformation.Should().Be(expectedResult);
        }
    }
}