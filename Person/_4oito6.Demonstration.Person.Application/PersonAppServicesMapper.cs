using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System.Linq;

namespace _4oito6.Demonstration.Person.Application
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    public static class PersonAppServicesMapper
    {
        private static Phone ToPhone(this PhoneRequest request)
        {
            return new Phone(request.Type, request.Code, request.Number);
        }

        private static Address ToAddress(this AddressRequest request)
        {
            return new Address(request.Street, request.Number, request.Complement, request.District, request.City, request.State, request.PostalCode);
        }

        public static Person ToPerson(this PersonRequest request, int id = 0)
        {
            if (id > 0)
            {
                return new Person
                (
                    id: id,
                    name: request.Name,
                    email: request.Email,

                    document: request.Document,
                    gender: request.Gender,

                    birthDate: request.BirthDate,
                    phones: request.Phones.Select(r => r.ToPhone()).ToList(),

                    mainPhone: request.MainPhone?.ToPhone(),
                    address: request.Address?.ToAddress()
                );
            }

            return new Person
            (
                name: request.Name,
                email: request.Email,

                document: request.Document,
                gender: request.Gender,

                birthDate: request.BirthDate,
                phones: request.Phones.Select(r => r.ToPhone()).ToList(),

                mainPhone: request.MainPhone?.ToPhone(),
                address: request.Address?.ToAddress()
            );
        }

        private static PhoneResponse ToPhoneResponse(this Phone phone)
        {
            return new PhoneResponse
            {
                Code = phone.Code,
                Id = phone.Id,
                Number = phone.Number,
                Type = phone.Type
            };
        }

        private static AddressResponse ToAddressResponse(this Address address)
        {
            return new AddressResponse
            {
                City = address.City,
                Complement = address.Complement,
                District = address.District,
                Id = address.Id,
                Number = address.Number,
                PostalCode = address.PostalCode,
                State = address.State,
                Street = address.Street
            };
        }

        public static PersonResponse ToPersonResponse(this Person person)
        {
            var response = new PersonResponse
            {
                Address = person.Address?.ToAddressResponse(),
                BirthDate = person.BirthDate,
                Document = person.Document,
                Gender = person.Gender,
                Email = person.Email,
                Id = person.Id,
                Name = person.Name,
                MainPhone = person.MainPhone.ToPhoneResponse()
            };

            response.Phones.AddRange(person.Phones.Select(p => p.ToPhoneResponse()));
            return response;
        }
    }
}