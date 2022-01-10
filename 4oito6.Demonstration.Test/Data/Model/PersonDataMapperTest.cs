using _4oito6.Demonstration.Data.Model;
using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Test.TestCases;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using System.Linq;
using Xunit;

namespace _4oito6.Demonstration.Test.Data.Model
{
    [Trait("PersonDataMapper", "Data Mapper tests")]
    public class PersonDataMapperTest
    {
        private readonly CompareLogic _comparison = new();

        [Fact(DisplayName = "Testing Phone's Domain Model's mapper")]
        public void ToPhone_Success()
        {
            //arrange:
            var dtos = PhoneTestCases.GetDtos();

            var expectedResult = dtos
                .Select(dto => new Phone(dto.phoneid, PersonDataMapper.PhoneTypes[dto.type], dto.code, dto.number))
                .ToList();

            //act:
            var result = dtos.Select(dto => dto.ToPhone()).ToList();

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [Fact(DisplayName = "Testing Address' Domain Model's mapper")]
        public void ToAddress_Success()
        {
            //arrange:
            var dtos = AddressTestCases.GetDtos();

            var expectedResult = dtos
                .Select(dto => new Address
                (
                    id: dto.addressid,
                    street: dto.street,

                    number: dto.number,
                    complement: dto.complement,

                    district: dto.district,
                    city: dto.city,

                    state: dto.state,
                    postalCode: dto.postalcode
                ))
                .ToList();

            //act:
            var result = dtos.Select(dto => dto.ToAddress()).ToList();

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [Fact(DisplayName = "Testing Person's split on's mapper")]
        public void MapPersonDtoProperties_Success()
        {
            //arrange:
            var phoneDto = PhoneTestCases.GetDtos(1).First();
            var addressDto = AddressTestCases.GetDtos(1).First();
            var dto = PersonTestCases.GetCompleteDto(phoneDto.phoneid, addressDto.addressid);

            var expectedResult = (CompletePersonDto)dto.Clone();
            expectedResult.Phone = (PhoneDto)phoneDto.Clone();
            expectedResult.Address = (AddressDto)addressDto.Clone();

            //act:
            var result = PersonDataMapper.MapPersonDtoProperties(dto, addressDto, phoneDto);

            //assert:
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [Fact(DisplayName = "Testing Person's Domain Model's mapper")]
        public void ToPerson_Success()
        {
            //arrange:
            var addressDto = AddressTestCases.GetDtos(1).First();
            var phoneDtos = PhoneTestCases.GetDtos();

            var mainPhoneId = phoneDtos.First().phoneid;
            var dto = PersonTestCases.GetDto(mainPhoneId, addressDto.addressid);

            var phones = phoneDtos.ToDictionary(x => x.phoneid, x => x.ToPhone());
            var address = addressDto?.ToAddress();

            var expectedResult = new Person
            (
                id: dto.personid,
                name: dto.name,
                email: dto.email,

                document: dto.document,
                gender: PersonDataMapper.Genders[dto.gender],
                birthDate: dto.birthdate,

                phones: phones.Values,
                mainPhone: phones[dto.mainphoneid],
                address: address
            );

            //act:
            var result = dto.ToPerson(addressDto, phoneDtos);

            //assert:
            expectedResult.Match(result).Should().BeTrue();
        }

        [Fact(DisplayName = "Testing Person's complete Domain Model's mapper")]
        public void ToPerson_Complete_Success()
        {
            //arrange:
            var addressDto = AddressTestCases.GetDtos(1).First();
            var phoneDtos = PhoneTestCases.GetDtos();
            var dtos = PersonTestCases.GetCompleteDto(phoneDtos, addressDto);

            var phones = phoneDtos.ToDictionary(x => x.phoneid, x => x.ToPhone());
            var address = addressDto?.ToAddress();

            var expectedResult = dtos.GroupBy(dto => dto.personid).Select(g => g.First())
                .Select
                (
                    dto => new Person
                    (
                        id: dto.personid,
                        name: dto.name,

                        email: dto.email,
                        document: dto.document,

                        gender: PersonDataMapper.Genders[dto.gender],
                        birthDate: dto.birthdate,

                        phones: dtos.Select(d => d.Phone.ToPhone()).ToList(),
                        mainPhone: dtos.Select(d => d.Phone).FirstOrDefault(d => d.phoneid == dto.mainphoneid).ToPhone(),
                        address: dto.Address?.ToAddress()
                    )
                ).FirstOrDefault();

            //act:
            var result = dtos.ToPerson();

            //assert:
            expectedResult.Match(result).Should().BeTrue();
        }
    }
}