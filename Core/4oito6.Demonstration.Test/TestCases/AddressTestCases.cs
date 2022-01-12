using _4oito6.Demonstration.Data.Model;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Test.TestCases
{
    internal static class AddressTestCases
    {
        internal static IEnumerable<AddressDto> GetDtos(int quantity = 5)
        {
            return new Faker<AddressDto>()
                .CustomInstantiator(f => new AddressDto
                {
                    addressid = f.Random.Int(1, 999),
                    city = f.Random.String(50),
                    complement = f.Random.String(50),

                    district = f.Random.String(50),
                    number = f.Random.Int(1, 9999).ToString(),
                    postalcode = f.Random.Int(11111111, 99999999).ToString(),

                    state = "RJ",
                    street = f.Random.String()
                })
                .Generate(quantity)
                .ToList();
        }
    }
}