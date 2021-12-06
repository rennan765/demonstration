using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Services;
using _4oito6.Demonstration.Contact.Domain.Services.Validators;
using _4oito6.Demonstration.Contact.Test.TestCases;
using _4oito6.Demonstration.Domain.Model.Entities;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.Contact.Test.Domain.Services
{
    [Trait("ContactServices", "Domain Services tests")]
    public class ContactServicesTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "MaintainContactInformationAsync_PersonNotFound")]
        public async Task MaintainContactInformationAsync_PersonNotFound()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<ContactServices>();
            var personId = 1;

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Person.GetByIdAsync(personId))
                .ReturnsAsync((Person)null)
                .Verifiable();

            //act:
            var expectedResult = Person.GetDefaultInstance();
            expectedResult.Validate(expectedResult, new PersonNotFoundValidator());
            var result = await services.MaintainContactInformationAsync(personId).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mocker.Verify();
        }

        [Fact(DisplayName = "MaintainContactInformationAsync_PersonNotFound")]
        public async Task MaintainContactInformationAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<ContactServices>();

            var person = PersonTestCases.GetPersons(1).First();
            var personId = person.Id;

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Person.GetByIdAsync(personId))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            var toUpdate = (Person)person.Clone();

            //arrange: address operations
            var address = (Address)person.Address.Clone();
            mocker.GetMock<IContactRepositoryRoot>()
                .Setup(r => r.Address.GetByAddressAsync(It.Is<Address>(a => _comparison.Compare(a, address).AreEqual)))
                .ReturnsAsync(address)
                .Verifiable();

            toUpdate.ClearAddress();
            toUpdate.Attach(address);

            //arrange: phone operations
            var phones = person.Phones.OrderBy(p => p.Code).ThenBy(p => p.Number)
                .ToDictionary(p => p.ToString(), p => (Phone)p.Clone());

            var mainPhone = (Phone)phones[person.MainPhone.ToString()].Clone();

            mocker.GetMock<IContactRepositoryRoot>()
                .Setup
                (
                    r => r.Phone.GetByNumberAsync
                    (
                        It.Is<IEnumerable<Phone>>
                        (
                            phns => _comparison
                                .Compare
                                (
                                    phns.OrderBy(person => person.ToString()).ToList(),
                                    person.Phones.OrderBy(person => person.ToString()).ToList()
                                )
                                .AreEqual
                        )
                    )
                )
                .ReturnsAsync(phones.Values)
                .Verifiable();

            toUpdate.ClearPhones();
            toUpdate.Attach(mainPhone, isMainPhone: true);

            toUpdate.Attach(phones.Values);
            toUpdate.ValidateToUpdate();

            //act:
            var expectedResult = (Person)toUpdate.Clone();
            var result = await services.MaintainContactInformationAsync(personId).ConfigureAwait(false);

            //assert:
            expectedResult.Match(result).Should().BeTrue();
            mocker.Verify();
        }
    }
}