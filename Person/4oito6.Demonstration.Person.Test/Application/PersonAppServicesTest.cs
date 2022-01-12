using _4oito6.Demonstration.Person.Application;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.Domain.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Services.Interfaces;
using _4oito6.Demonstration.Person.Test.TestCases;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace _4oito6.Demonstration.Person.Test.Application
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    [Trait("PersonAppServices", "Application tests")]
    public class PersonAppServicesTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "Handling null request while attempting to create a new Person.")]
        public async Task CreateAsync_ArgumentNull()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();

            await appService.Invoking(app => app.CreateAsync(null))
                .Should().ThrowAsync<ArgumentNullException>()
                .ConfigureAwait(false);
        }

        [Fact(DisplayName = "Creating a Person successfully.")]
        public async Task CreateAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();
            var request = PersonTestCases.GetRequests(1).First();

            //arrange: attempting to create
            var person = request.ToPerson();
            var newPerson = request.ToPerson(50);

            mocker.GetMock<IPersonServices>()
                .Setup(p => p.CreateAsync(It.Is<Person>(prsn => person.Match(prsn))))
                .ReturnsAsync(newPerson)
                .Verifiable();

            //arrange: persisting
            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.Commit())
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = newPerson.ToPersonResponse();
            var result = await appService.CreateAsync(request).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            mocker.Verify();
        }

        [Fact(DisplayName = "Attempting to create a new Person, but entries are invalid.")]
        public async Task CreateAsync_BadRequest()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();
            var request = new PersonRequest();

            //arrange: attempting to create
            var person = request.ToPerson();
            mocker.GetMock<IPersonServices>()
                .Setup(p => p.CreateAsync(It.Is<Person>(prsn => person.Match(prsn))))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //arrange: persisting
            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.Rollback())
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = person.ToPersonResponse();
            var result = await appService.CreateAsync(request).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            mocker.Verify();
        }

        [Fact(DisplayName = "Attempting to get a Person by its e-mail.")]
        public async Task GetByEmailAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();
            var person = PersonTestCases.GetPersons(1).First();

            mocker.GetMock<IPersonServices>()
                .Setup(s => s.GetByEmailAsync(person.Email))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = person.ToPersonResponse();
            var result = await appService.GetByEmailAsync(person.Email).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            mocker.Verify();
        }

        [Fact(DisplayName = "Attempting to get a Person by its id.")]
        public async Task GetByIdAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();
            var id = 10;

            mocker.GetMock<IPersonServices>()
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((Person)null)
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = (PersonResponse)null;
            var result = await appService.GetByIdAsync(id).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            mocker.Verify();
        }

        [Fact(DisplayName = "Handling null request while attempting to update a new Person.")]
        public async Task UpdateAsync_ArgumentNull()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();

            await appService.Invoking(app => app.UpdateAsync(0, null))
                .Should().ThrowAsync<ArgumentNullException>()
                .ConfigureAwait(false);
        }

        [Fact(DisplayName = "Updating a Person successfully.")]
        public async Task UpdateAsync_Success()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();

            var id = 10;
            var request = PersonTestCases.GetRequests(1).First();

            //arrange: attempting to update
            var person = request.ToPerson(id);
            mocker.GetMock<IPersonServices>()
                .Setup(p => p.UpdateAsync(It.Is<Person>(prsn => person.Match(prsn))))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //arrange: persisting
            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.Commit())
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = person.ToPersonResponse();
            var result = await appService.UpdateAsync(id, request).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            mocker.Verify();
        }

        [Fact(DisplayName = "Attempting to update a new Person, but entries are invalid.")]
        public async Task UpdateAsync_BadRequest()
        {
            //arrange:
            var mocker = new AutoMocker();
            var appService = mocker.CreateInstance<PersonAppServices>();
            var id = 10;
            var request = new PersonRequest();

            //arrange: attempting to update
            var person = request.ToPerson(id);
            mocker.GetMock<IPersonServices>()
                .Setup(p => p.UpdateAsync(It.Is<Person>(prsn => person.Match(prsn))))
                .ReturnsAsync((Person)person.Clone())
                .Verifiable();

            //arrange: persisting
            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.Rollback())
                .Verifiable();

            mocker.GetMock<IPersonUnitOfWork>()
                .Setup(u => u.CloseConnections())
                .Verifiable();

            //act:
            var expectedResult = person.ToPersonResponse();
            var result = await appService.UpdateAsync(id, request).ConfigureAwait(false);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            appService.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            mocker.Verify();
        }
    }
}