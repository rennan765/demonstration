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
        private readonly CompareLogic _compariton = new CompareLogic();

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

            mocker.GetMock<IPersonRepositoryRoot>()
                .Setup
                (
                    r => r.Person.InsertAsync
                    (
                        It.Is<Person>(p => _compariton.Compare(p, person).AreEqual)
                    )
                )
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //act:
            var expectedResult = (Person)person.Clone();
            var result = await services.CreateAsync(person).ConfigureAwait(false);

            //assert:
            _compariton.Compare(expectedResult, result).AreEqual.Should().BeTrue();
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
    }
}