using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Data.Model
{
    public static class Mapper
    {
        private static Dictionary<int, Gender> _genders;
        private static Dictionary<int, PhoneType> _phoneTypes;

        static Mapper()
        {
            _genders = new Dictionary<int, Gender>
            {
                { (int)Gender.Male, Gender.Male },
                { (int)Gender.Female, Gender.Female },
                { (int)Gender.NotInformed, Gender.NotInformed }
            };

            _phoneTypes = new Dictionary<int, PhoneType>
            {
                { (int)PhoneType.Cel, PhoneType.Cel },
                { (int)PhoneType.Business, PhoneType.Business },
                { (int)PhoneType.Home, PhoneType.Home }
            };
        }

        public static Phone ToPhone(this PhoneDto dto)
        {
            return new Phone(dto.Id, _phoneTypes[dto.Type], dto.Code, dto.Number);
        }

        public static PhoneDto ToPhoneDto(this Phone phone)
        {
            return new PhoneDto
            {
                Code = phone.Code,
                Number = phone.Number,
                Id = phone.Id,
                Type = (int)phone.Type
            };
        }

        public static Address ToAddress(this AddressDto dto)
        {
            return new Address
            (
                id: dto.Id,
                street: dto.Street,

                number: dto.Number,
                complement: dto.Complement,

                district: dto.District,
                city: dto.City,
                state: dto.State
            );
        }

        public static AddressDto ToAddress(this Address address)
        {
            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                Number = address.Number,
                City = address.City,
                State = address.State,
                Complement = address.Complement,
                District = address.District
            };
        }

        public static CompletePersonDto MapPersonDtoProperties(CompletePersonDto person, AddressDto address, PhoneDto phone)
        {
            person.Address = address;
            person.Phone = phone;
            return person;
        }

        public static Person ToPerson(this IEnumerable<CompletePersonDto> dtos)
        {
            return dtos.GroupBy(dto => dto.Id).Select(g => g.First())
                .Select
                (
                    dto => new Person
                    (
                        id: dto.Id,
                        name: dto.Name,

                        document: dto.Document,
                        gender: _genders[dto.Gender],
                        birthDate: dto.BirthDate,

                        phones: dtos.Select(d => d.Phone.ToPhone()).ToList(),
                        mainPhone: dtos.Select(d => d.Phone).FirstOrDefault(d => d.Id == dto.MainPhoneId).ToPhone(),
                        address: dto.Address?.ToAddress()
                    )
                ).FirstOrDefault();
        }
    }
}