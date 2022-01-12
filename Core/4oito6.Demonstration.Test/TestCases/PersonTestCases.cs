using _4oito6.Demonstration.Data.Model;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Test.TestCases
{
    internal static class PersonTestCases
    {
        internal static PersonDto GetDto(int mainPhoneId, int? addressId = null)
        {
            return new Faker<PersonDto>()
                .CustomInstantiator(f => new PersonDto
                {
                    addressid = addressId,
                    birthdate = DateTime.UtcNow.Date,
                    document = "98606505005",

                    email = "my@gmail.com",
                    gender = f.PickRandom(PersonDataMapper.Genders.Select(t => t.Key)),
                    mainphoneid = mainPhoneId,

                    name = f.Random.String(20),
                    personid = f.Random.Int(1, 999)
                })
                .Generate(1).First();
        }

        internal static CompletePersonDto GetCompleteDto(int mainPhoneId, int? addressId = null)
        {
            return new Faker<CompletePersonDto>()
                .CustomInstantiator(f => new CompletePersonDto
                {
                    addressid = addressId,
                    birthdate = DateTime.UtcNow.Date,
                    document = "98606505005",

                    email = "my@gmail.com",
                    gender = f.PickRandom(PersonDataMapper.Genders.Select(t => t.Key)),
                    mainphoneid = mainPhoneId,

                    name = f.Random.String(20),
                    personid = f.Random.Int(1, 999)
                })
                .Generate(1).First();
        }

        internal static IEnumerable<CompletePersonDto> GetCompleteDto(IEnumerable<PhoneDto> phoneDtos, AddressDto addressDto)
        {
            var faker = new Faker<CompletePersonDto>();
            var mainPhoneId = phoneDtos.First().phoneid;

            return phoneDtos
                .Select(phoneDto =>
                {
                    var dto = faker
                        .CustomInstantiator(f => new CompletePersonDto
                        {
                            addressid = addressDto.addressid,
                            birthdate = DateTime.UtcNow.Date,
                            document = "98606505005",

                            email = "my@gmail.com",
                            gender = f.PickRandom(PersonDataMapper.Genders.Select(t => t.Key)),
                            mainphoneid = mainPhoneId,

                            name = f.Random.String(20),
                            personid = f.Random.Int(1, 999)
                        })
                        .Generate(1).First();

                    dto.Phone = phoneDto;
                    dto.Address = addressDto;
                    return dto;
                })
                .ToList();
        }
    }
}