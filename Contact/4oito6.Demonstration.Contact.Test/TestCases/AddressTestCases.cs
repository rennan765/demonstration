using _4oito6.Demonstration.Domain.Model.Entities;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Contact.Test.TestCases
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
    }
}