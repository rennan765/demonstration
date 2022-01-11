using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Services;
using _4oito6.Demonstration.Contact.Test.TestCases;
using _4oito6.Demonstration.Domain.Model.Entities;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.Contact.Test.Domain.Services
{
    [Trait("CloningServices", "Domain Services tests")]
    public class CloningServicesTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "Cloning entities successfully.")]
        public async Task CloneAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var service = mocker.CreateInstance<CloningServices>();

            //arrange: address operation
            var addresses = AddressTestCases.GetAddresses();
            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Address.GetAllAsync())
                .ReturnsAsync(addresses)
                .Verifiable();

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Address.CloneAsync(It.Is<IEnumerable<Address>>(adds => _comparison.Compare(adds, addresses).AreEqual)))
                .Verifiable();

            //arrange: phone operation
            var phones = PhoneTestCases.GetPhones();
            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Phone.GetAllAsync())
                .ReturnsAsync(phones)
                .Verifiable();

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Phone.CloneAsync(It.Is<IEnumerable<Phone>>(phs => _comparison.Compare(phs, phones).AreEqual)))
                .Verifiable();

            //arrange: person operation
            var persons = PersonTestCases.GetPersons();
            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Person.GetAllAsync())
                .ReturnsAsync(persons)
                .Verifiable();

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Person.CloneAsync(It.Is<IEnumerable<Person>>(prs => _comparison.Compare(prs, persons).AreEqual)))
                .Verifiable();

            //act:
            await service.CloneAsync().ConfigureAwait(false);

            //assert:
            mocker.Verify();
        }
    }
}