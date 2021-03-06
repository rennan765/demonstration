using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace _4oito6.Demonstration.Data.Model
{
    public static class PersonDataMapper
    {
        public static Dictionary<int, Gender> Genders;
        public static Dictionary<int, PhoneType> PhoneTypes;

        static PersonDataMapper()
        {
            Genders = new Dictionary<int, Gender>
            {
                { (int)Gender.Male, Gender.Male },
                { (int)Gender.Female, Gender.Female },
                { (int)Gender.NotInformed, Gender.NotInformed }
            };

            PhoneTypes = new Dictionary<int, PhoneType>
            {
                { (int)PhoneType.Cel, PhoneType.Cel },
                { (int)PhoneType.Business, PhoneType.Business },
                { (int)PhoneType.Home, PhoneType.Home }
            };
        }

        public static Phone ToPhone(this PhoneDto dto)
        {
            return new Phone(dto.phoneid, PhoneTypes[dto.type], dto.code, dto.number);
        }

        [ExcludeFromCodeCoverage]
        public static PhoneDto ToPhoneDto(this Phone phone)
        {
            return new PhoneDto
            {
                code = phone.Code,
                number = phone.Number,
                phoneid = phone.Id,
                type = (int)phone.Type
            };
        }

        public static Address ToAddress(this AddressDto dto)
        {
            return new Address
            (
                id: dto.addressid,
                street: dto.street,

                number: dto.number,
                complement: dto.complement,

                district: dto.district,
                city: dto.city,

                state: dto.state,
                postalCode: dto.postalcode
            );
        }

        [ExcludeFromCodeCoverage]
        public static AddressDto ToAddressDto(this Address address)
        {
            return new AddressDto
            {
                addressid = address.Id,
                street = address.Street,
                number = address.Number,
                city = address.City,
                state = address.State,
                complement = address.Complement,
                district = address.District,
                postalcode = address.PostalCode
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
            return dtos.GroupBy(dto => dto.personid).Select(g => g.First())
                .Select
                (
                    dto => new Person
                    (
                        id: dto.personid,
                        name: dto.name,

                        email: dto.email,
                        document: dto.document,

                        gender: Genders[dto.gender],
                        birthDate: dto.birthdate,

                        phones: dtos.Select(d => d.Phone.ToPhone()).ToList(),
                        mainPhone: dtos.Select(d => d.Phone).FirstOrDefault(d => d.phoneid == dto.mainphoneid).ToPhone(),
                        address: dto.Address?.ToAddress()
                    )
                ).FirstOrDefault();
        }

        public static Person ToPerson(this PersonDto dto, AddressDto? addressDto, IEnumerable<PhoneDto> phoneDtos)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var phones = phoneDtos.ToDictionary(x => x.phoneid, x => x.ToPhone());
            var address = addressDto?.ToAddress();

            return new Person
            (
                id: dto.personid,
                name: dto.name,
                email: dto.email,

                document: dto.document,
                gender: Genders[dto.gender],
                birthDate: dto.birthdate,

                phones: phones.Values,
                mainPhone: phones[dto.mainphoneid],
                address: address
            );
        }

        [ExcludeFromCodeCoverage]
        public static PersonDto ToPersonDto(this Person person)
        {
            return new PersonDto
            {
                addressid = person.Address?.Id,
                birthdate = person.BirthDate,
                document = person.Document,
                personid = person.Id,
                name = person.Name,
                gender = (int)person.Gender,
                mainphoneid = person.MainPhone.Id,
                email = person.Email
            };
        }
    }
}