using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Services;
using _4oito6.Demonstration.Person.Test.TestCases;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.Person.Test.Domain.Service
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    [Trait("PersonServices", "Domain Services tests")]
    public class PersonServicesTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "CreateAsync_ArgumentNullException")]
        public async Task CreateAsync_ArgumentNullException()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();

            //act and assert:
            await services.Invoking(s => s.CreateAsync(null))
                .Should().ThrowAsync<ArgumentNullException>()
                .ConfigureAwait(false);
        }

        [Fact(DisplayName = "CreateAsync_Conflict")]
        public async Task CreateAsync_Conflict()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByEmailOrDocumentAsync(person.Email, person.Document))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //act:
            var result = await services.CreateAsync(person).ConfigureAwait(false);

            //assert:
            result.IsValid.Should().BeFalse();
            mocker.Verify();
        }

        [Fact(DisplayName = "CreateAsync_Invalid")]
        public async Task CreateAsync_Invalid()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();

            var phones = PhoneTestCases.GetPhones();

            var person = new Person
            (
                name: string.Empty,
                email: "email inválido",

                document: "7777",
                gender: Gender.NotInformed,
                birthDate: DateTime.UtcNow.Date,

                phones: phones,
                mainPhone: phones.FirstOrDefault()
            );

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByEmailOrDocumentAsync(person.Email, person.Document))
                .ReturnsAsync((Person)null)
                .Verifiable();

            //act:
            var result = await services.CreateAsync(person).ConfigureAwait(false);

            //assert:
            result.IsValid.Should().BeFalse();
            mocker.Verify();
        }

        [Fact(DisplayName = "CreateAsync_Success")]
        public async Task CreateAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByEmailOrDocumentAsync(person.Email, person.Document))
                .ReturnsAsync((Person)null)
                .Verifiable();

            var inserted = (Person)person.Clone();
            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup
                (
                    r => r.Person.InsertAsync
                    (
                        It.Is<Person>(p => _comparison.Compare(p, person).AreEqual)
                    )
                )
                .ReturnsAsync(inserted)
                .Verifiable();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup
                (
                    r => r.Person.RequestMaintainContactInformationAsync
                    (
                        It.Is<Person>(p => _comparison.Compare(p, inserted).AreEqual)
                    )
                )
                .Verifiable();

            //act:
            var expectedResult = (Person)inserted.Clone();
            var result = await services.CreateAsync(person).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mocker.Verify();
        }

        [Fact(DisplayName = "GetByEmailAsync_Success")]
        public async Task GetByEmailAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByEmailAsync(person.Email))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //act:
            var expectedResult = (Person)person.Clone();
            var result = await services.GetByEmailAsync(person.Email).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mocker.Verify();
        }

        [Fact(DisplayName = "GetByIdAsync_Success")]
        public async Task GetByIdAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByIdAsync(person.Id))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //act:
            var expectedResult = (Person)person.Clone();
            var result = await services.GetByIdAsync(person.Id).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mocker.Verify();
        }

        [Fact(DisplayName = "UpdateAsync_ArgumentNullException")]
        public async Task UpdateAsync_ArgumentNullException()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();

            //act and assert:
            await services.Invoking(s => s.UpdateAsync(null))
                .Should().ThrowAsync<ArgumentNullException>()
                .ConfigureAwait(false);
        }

        [Fact(DisplayName = "UpdateAsync_NotFound")]
        public async Task UpdateAsync_NotFound()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByIdAsync(person.Id))
                .ReturnsAsync((Person)null)
                .Verifiable();

            //act:
            var result = await services.UpdateAsync(person).ConfigureAwait(false);

            //assert:
            result.IsValid.Should().BeFalse();
            mocker.Verify();
        }

        [Fact(DisplayName = "UpdateAsync_Success")]
        public async Task UpdateAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var services = mocker.CreateInstance<PersonServices>();
            var person = PersonTestCases.GetPersons(1).FirstOrDefault();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup(r => r.Person.GetByIdAsync(person.Id))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            var toUpdate = (Person)person.Clone();
            toUpdate.ValidateToUpdate();
            var updated = (Person)toUpdate.Clone();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup
                (
                    r => r.Person.UpdateAsync
                    (
                        It.Is<Person>(p => _comparison.Compare(p, toUpdate).AreEqual)
                    )
                )
                .ReturnsAsync(updated)
                .Verifiable();

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup
                (
                    r => r.Person.RequestMaintainContactInformationAsync
                    (
                        It.Is<Person>(p => _comparison.Compare(p, updated).AreEqual)
                    )
                )
                .Verifiable();

            //act:
            var expectedResult = (Person)updated.Clone();
            var result = await services.UpdateAsync(person).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mocker.Verify();
        }
    }
}