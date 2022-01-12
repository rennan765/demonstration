using _4oito6.Demonstration.Data.Model;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Test.TestCases
{
    internal static class PhoneTestCases
    {
        internal static IEnumerable<PhoneDto> GetDtos(int quantity = 5)
        {
            return new Faker<PhoneDto>()
                .CustomInstantiator(f => new PhoneDto
                {
                    code = f.Random.Int(11, 99).ToString(),
                    number = f.Random.Int(11111111, 99999999).ToString(),

                    phoneid = f.Random.Int(1, 999),
                    type = f.PickRandom(PersonDataMapper.PhoneTypes.Select(t => t.Key))
                })
                .Generate(quantity)
                .ToList();
        }
    }
}