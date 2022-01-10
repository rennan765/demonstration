using _4oito6.Demonstration.Person.Application;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using _4oito6.Demonstration.Person.Test.TestCases;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace _4oito6.Demonstration.Person.Test.Application
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    [Trait("PersonAppServicesMapper", "Service Mapper tests")]
    public class PersonAppServicesMapperTest
    {
        [Theory(DisplayName = "Testing if request is successfully mapped to Domain Model.")]
        [InlineData(0)]
        [InlineData(1)]
        public void ToPerson_Success(int id)
        {
            //arrange:
            var request = PersonTestCases.GetRequests(1).First();

            Person expectedResult = null;

            if (id > 0)
            {
                expectedResult = new Person
                (
                    id: id,
                    name: request.Name,
                    email: request.Email,

                    document: request.Document,
                    gender: request.Gender,

                    birthDate: request.BirthDate,
                    phones: request.Phones
                        .Select(r => new Phone(r.Type, r.Code, r.Number))
                        .ToList(),

                    mainPhone: new Phone
                    (
                        type: request.MainPhone.Type,
                        code: request.MainPhone.Code,
                        number: request.MainPhone.Number
                    ),

                    address: new Address
                    (
                        street: request.Address.Street,
                        number: request.Address.Number,
                        complement: request.Address.Complement,

                        district: request.Address.District,
                        city: request.Address.City,

                        state: request.Address.State,
                        postalCode: request.Address.PostalCode
                    )
                );
            }
            else
            {
                expectedResult = new Person
                (
                    name: request.Name,
                        email: request.Email,

                        document: request.Document,
                        gender: request.Gender,

                        birthDate: request.BirthDate,
                        phones: request.Phones
                            .Select(r => new Phone(r.Type, r.Code, r.Number))
                            .ToList(),

                        mainPhone: new Phone
                        (
                            type: request.MainPhone.Type,
                            code: request.MainPhone.Code,
                            number: request.MainPhone.Number
                        ),

                        address: new Address
                        (
                            street: request.Address.Street,
                            number: request.Address.Number,
                            complement: request.Address.Complement,

                            district: request.Address.District,
                            city: request.Address.City,

                            state: request.Address.State,
                            postalCode: request.Address.PostalCode
                        )
                );
            }

            //act:
            var result = request.ToPerson(id);

            //assert:
            expectedResult.Match(result).Should().BeTrue();
        }

        [Fact(DisplayName = "Testing if Domain Model is successfully mapped to Service Model.")]
        public void ToPersonResponse_Success()
        {
            //arrange:
            var person = PersonTestCases.GetPersons(1).First();

            var expectedResult = new PersonResponse
            {
                Address = new AddressResponse
                {
                    City = person.Address.City,
                    Complement = person.Address.Complement,
                    District = person.Address.District,
                    Id = person.Address.Id,
                    Number = person.Address.Number,
                    PostalCode = person.Address.PostalCode,
                    State = person.Address.State,
                    Street = person.Address.Street
                },

                BirthDate = person.BirthDate,
                Document = person.Document,
                Gender = person.Gender,
                Email = person.Email,
                Id = person.Id,
                Name = person.Name,

                MainPhone = new PhoneResponse
                {
                    Code = person.MainPhone.Code,
                    Id = person.MainPhone.Id,
                    Number = person.MainPhone.Number,
                    Type = person.MainPhone.Type
                }
            };

            expectedResult.Phones
                .AddRange(person
                    .Phones.Select(p => new PhoneResponse
                    {
                        Code = p.Code,
                        Id = p.Id,
                        Number = p.Number,
                        Type = p.Type
                    }));

            //act:
            var result = person.ToPersonResponse();

            //assert:
            expectedResult.Match(result).Should().BeTrue();
        }
    }
}