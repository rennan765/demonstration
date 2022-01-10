using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Person.Application.Model.Address;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Person.Test.TestCases
{
    internal static class AddressTestCases
    {
        internal static IEnumerable<Address> GetAddresses(int quantity = 5)
        {
            return new Faker<Address>()
                .CustomInstantiator
                (
                    f => new Address
                    (
                        id: f.Random.Int(1, 100),
                        street: f.Random.String(),
                        number: f.Random.Int(1000, 9999).ToString(),
                        complement: null,
                        district: f.Random.String(),
                        city: f.Random.String(),
                        state: f.Random.String(length: 2),
                        postalCode: f.Random.Long(10000000, 99999999).ToString()
                    )
                )
                .Generate(quantity)
                .ToList();
        }

        internal static AddressRequest GetRequest()
        {
            return new Faker<AddressRequest>()
                .CustomInstantiator(f => new AddressRequest
                {
                    Street = f.Random.String(),
                    Number = f.Random.Int(1000, 9999).ToString(),
                    Complement = null,
                    District = f.Random.String(),
                    City = f.Random.String(),
                    State = f.Random.String(length: 2),
                    PostalCode = f.Random.Long(10000000, 99999999).ToString()
                })
                .Generate(1)
                .First();
        }
    }
}