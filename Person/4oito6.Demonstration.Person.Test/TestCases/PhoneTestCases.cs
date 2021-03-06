using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Person.Test.TestCases
{
    internal static class PhoneTestCases
    {
        internal static IEnumerable<Phone> GetPhones(int quantity = 5)
        {
            return new Faker<Phone>()
                .CustomInstantiator
                (
                    f => new Phone
                    (
                        id: f.Random.Int(1, 100),
                        type: f.PickRandom<PhoneType>(),

                        code: f.Random.Int(11, 99).ToString(),
                        number: f.Random.Long(10000000, 999999999).ToString()
                    )
                )
                .Generate(quantity)
                .ToList();
        }

        internal static IEnumerable<PhoneRequest> GetRequests(int quantity = 5)
        {
            return new Faker<PhoneRequest>()
                .CustomInstantiator(f => new PhoneRequest
                {
                    Type = f.PickRandom<PhoneType>(),
                    Code = f.Random.Int(11, 99).ToString(),
                    Number = f.Random.Long(10000000, 999999999).ToString()
                })
                .Generate(quantity)
                .ToList();
        }
    }
}