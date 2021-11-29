using _4oito6.Demonstration.AuditTrail.EntryPoint.Domain.Data;
using _4oito6.Demonstration.AuditTrail.EntryPoint.Domain.Services;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.AuditTrail.Test.Domain.Services
{
    [Trait("AuditTrailServices", "Domain Services tests")]
    public class AuditTrailServicesTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "ProcessAsync_ArgumentNullException")]
        public async Task ProcessAsync_ArgumentNullException()
        {
            //arrange:
            var mocker = new AutoMocker();
            var service = mocker.CreateInstance<AuditTrailServices>();

            await service.Invoking(s => s.ProcessAsync(null))
                .Should().ThrowAsync<ArgumentNullException>()
                .ConfigureAwait(false);
        }

        [Fact(DisplayName = "ProcessAsync_DefaultCode")]
        public async Task ProcessAsync_DefaultCode()
        {
            //arrange:
            var mocker = new AutoMocker();
            var service = mocker.CreateInstance<AuditTrailServices>();

            var auditTrail = new AuditTrailMessage
            (
                id: Guid.NewGuid(),
                code: string.Empty,
                message: string.Empty,
                date: DateTime.UtcNow.Date
            );

            var toInsert = (AuditTrailMessage)auditTrail.Clone();
            toInsert.Code = AuditTrailServices.DefautlCode;
            toInsert.Message += AuditTrailServices.DefaultMessage;

            mocker.GetMock<IAuditTrailRepository>()
                .Setup(r => r.InsertAsync(It.Is<AuditTrailMessage>(atm => _comparison.Compare(atm, toInsert).AreEqual)))
                .Verifiable();

            //act:
            await service.ProcessAsync(auditTrail).ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }

        [Fact(DisplayName = "ProcessAsync_Success")]
        public async Task ProcessAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var service = mocker.CreateInstance<AuditTrailServices>();

            var auditTrail = new AuditTrailMessage
            (
                id: Guid.NewGuid(),
                code: "TESTE_IN01",
                message: string.Empty,
                date: DateTime.UtcNow.Date
            );

            mocker.GetMock<IAuditTrailRepository>()
                .Setup(r => r.InsertAsync(It.Is<AuditTrailMessage>(atm => _comparison.Compare(atm, auditTrail).AreEqual)))
                .Verifiable();

            //act:
            await service.ProcessAsync(auditTrail).ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }
    }
}