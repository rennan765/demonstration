using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Contact.Application;
using _4oito6.Demonstration.Contact.Application.Arguments;
using _4oito6.Demonstration.Contact.Domain.Data.Transaction;
using _4oito6.Demonstration.Contact.Domain.Services.Interfaces;
using _4oito6.Demonstration.Contact.Domain.Services.Validators;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.SQS.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.Contact.Test.Application
{
    [Trait("ContactAppServices", "Application tests")]
    public class ContactAppServicesTest
    {
        [Fact(DisplayName = "Person's information maintained successfully.")]
        public async Task MaintainInformationAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<ContactAppServices>();
            var personId = 1;

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.EnableTransactions())
                .Verifiable();

            //arrange: attempting to maintain:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: personId,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },

                mainPhone: phone,
                address: new Address
                (
                    street: "Avenida Rio Branco",

                    number: "156",
                    complement: null,

                    district: "Centro",
                    city: "Rio de Janeiro",

                    state: "RJ",
                    postalCode: "20040901"
                )
            );

            mocker.GetMock<IContactServices>()
                .Setup(s => s.MaintainContactInformationAsync(personId))
                .ReturnsAsync(person)
                .Verifiable();

            //arrange: maintained successfully
            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.Commit())
                .Verifiable();

            var message = new AuditTrailMessage
            (
                code: ContactAppServices.MaintainInformationMessage,
                message: "Informações de contato atualizadas com sucesso.",
                additionalInformation: $"PhoneIds: {string.Join(",", person.Phones.Select(p => p.Id))}. MainPhoneId: {person.MainPhone.Id}. AddressId: {person.Address.Id}. "
            );

            mocker.GetMock<IAuditTrailSender>()
                .Setup(s => s.SendAsync(It.Is<AuditTrailMessage>(m => m.Match(message, false))))
                .Verifiable();

            //act:
            await appService.MaintainInformationAsync(personId).ConfigureAwait(false);

            //assert:
            appService.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            mocker.Verify();
        }

        [Fact(DisplayName = "Failure to maintain Person's information.")]
        public async Task MaintainInformationAsync_Failure()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<ContactAppServices>();
            var personId = 1;

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.EnableTransactions())
                .Verifiable();

            //arrange: attempting to maintain:
            var person = Person.GetDefaultInstance();
            person.Validate(person, new PersonNotFoundValidator());

            mocker.GetMock<IContactServices>()
                .Setup(s => s.MaintainContactInformationAsync(personId))
                .ReturnsAsync(person)
                .Verifiable();

            //arrange: failure to maintain
            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.Rollback())
                .Verifiable();

            var notifications = person.ValidationResults.
                    SelectMany(r => r.Errors)
                    .Select(vf => new Notification(vf.PropertyName, vf.ErrorMessage));

            var message = new AuditTrailMessage
            (
                code: ContactAppServices.WarningMessage,
                message: "Falha ao realizar a manutenção das informações de contato.",
                additionalInformation: JsonConvert.SerializeObject(notifications)
            );

            mocker.GetMock<IAuditTrailSender>()
                .Setup(s => s.SendAsync(It.Is<AuditTrailMessage>(m => m.Match(message, false))))
                .Verifiable();

            //act:
            await appService.MaintainInformationAsync(personId).ConfigureAwait(false);

            //assert:
            appService.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            mocker.Verify();
        }

        [Fact(DisplayName = "Maintain Person's information by id successfully.")]
        public async Task MaintainInformationByPersonIdAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var personId = 10;

            appServiceMock
                .Setup(app => app.MaintainInformationAsync(personId))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.MaintainInformationByPersonIdAsync(personId).ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Handling Exception while attepting to maintain Person's information by id.")]
        public async Task MaintainInformationByPersonIdAsync_Exception()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var personId = 10;
            appServiceMock
                .Setup(app => app.MaintainInformationAsync(personId))
                .ThrowsAsync(new InvalidOperationException())
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<InvalidOperationException>(), null))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.MaintainInformationByPersonIdAsync(personId).ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Attempting to maintain Person's information by queue, but it's empty.")]
        public async Task MaintainInformationByQueueAsync_Empty()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var request = new MaintainInformationRequest
            {
                PersonId = 10
            };

            mocker.GetMock<ISQSHelper>()
                .Setup(h => h.GetAsync<MaintainInformationRequest>())
                .ReturnsAsync((MaintainInformationRequest)null)
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleEmptyQueue("Empty queue."))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.MaintainInformationByQueueAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Maintain Person's information by queue successfully.")]
        public async Task MaintainInformationByQueueAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var request = new MaintainInformationRequest
            {
                PersonId = 10
            };

            mocker.GetMock<ISQSHelper>()
                .Setup(h => h.GetAsync<MaintainInformationRequest>())
                .ReturnsAsync(request)
                .Verifiable();

            appServiceMock
                .Setup(app => app.MaintainInformationAsync(request.PersonId))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.MaintainInformationByQueueAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Handling Exception while attepting to maintain Person's information by queue.")]
        public async Task MaintainInformationByQueueAsync_Exception()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            var request = new MaintainInformationRequest
            {
                PersonId = 10
            };

            mocker.GetMock<ISQSHelper>()
                .Setup(h => h.GetAsync<MaintainInformationRequest>())
                .ReturnsAsync(request)
                .Verifiable();

            appServiceMock
                .Setup(app => app.MaintainInformationAsync(request.PersonId))
                .ThrowsAsync(new InvalidOperationException())
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<InvalidOperationException>(), null))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.MaintainInformationByQueueAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }

        [Fact(DisplayName = "Clone executed successfully.")]
        public async Task CloneAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<ContactAppServices>();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.EnableTransactions())
                .Verifiable();

            mocker.GetMock<ICloningServices>()
                .Setup(s => s.CloneAsync())
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.Commit())
                .Verifiable();

            mocker.GetMock<IAuditTrailSender>()
                .Setup
                (
                    s => s.SendAsync
                    (
                        ContactAppServices.CloneInformationMessage,
                        It.Is<string>(str => str.Contains("Processamento de clone finalizado na data UTC")),
                        null
                    )
                )
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appService.CloneAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }

        [Fact(DisplayName = "Handling Exception while attepting to clone.")]
        public async Task CloneAsync_Exception()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appServiceMock = new Mock<ContactAppServices>
            (
                mocker.GetMock<IContactServices>().Object,
                mocker.GetMock<ICloningServices>().Object,

                mocker.GetMock<IContactUnitOfWork>().Object,
                mocker.GetMock<ISQSHelper>().Object,

                mocker.GetMock<ILogger<ContactAppServices>>().Object,
                mocker.GetMock<IAuditTrailSender>().Object
            );

            mocker.GetMock<ICloningServices>()
                .Setup(app => app.CloneAsync())
                .ThrowsAsync(new InvalidOperationException())
                .Verifiable();

            appServiceMock
                .Setup(app => app.HandleExceptionAsync(It.IsAny<InvalidOperationException>(), null))
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.Rollback())
                .Verifiable();

            mocker.GetMock<IContactUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            await appServiceMock.Object.CloneAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
            Mock.Verify(appServiceMock);
        }
    }
}