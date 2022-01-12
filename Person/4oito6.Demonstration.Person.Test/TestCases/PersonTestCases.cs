using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Person;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Person.Test.TestCases
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    internal static class PersonTestCases
    {
        internal static IEnumerable<Person> GetPersons(int quantity = 5)
        {
            var phones = PhoneTestCases.GetPhones();

            return new Faker<Person>()
                .CustomInstantiator
                (
                    f => new Person
                    (
                        id: f.Random.Int(1, 100),
                        name: f.Random.String(),

                        email: "my@gmail.com",
                        document: "14192684063",

                        gender: f.PickRandom<Gender>(),
                        birthDate: DateTime.UtcNow.AddYears(-20),

                        phones: phones.Select(p => (Phone)p.Clone()),
                        mainPhone: (Phone)phones.FirstOrDefault().Clone(),
                        address: AddressTestCases.GetAddresses(1).FirstOrDefault()
                    )
                )
                .Generate(quantity)
                .ToList();
        }

        internal static IEnumerable<PersonRequest> GetRequests(int quantity = 5)
        {
            return new Faker<PersonRequest>()
                .CustomInstantiator(f =>
                {
                    var phones = PhoneTestCases.GetRequests();
                    var request = new PersonRequest
                    {
                        BirthDate = DateTime.UtcNow.AddYears(-20),
                        Document = "14192684063",
                        Email = "my@gmail.com",
                        Gender = f.PickRandom<Gender>(),
                        Name = f.Random.String(),
                        MainPhone = f.PickRandom(phones),
                        Phones = phones,
                        Address = AddressTestCases.GetRequest()
                    };

                    return request;
                })
                .Generate(quantity)
                .ToList();
        }
    }
}