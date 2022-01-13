using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace _4oito6.Demonstration.Person.Application.Model.Person
{
    [ExcludeFromCodeCoverage]
    public class PersonRequest : ICloneable
    {
        public string Name { get; set; }

        public string Document { get; set; }

        public string Email { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public PhoneRequest MainPhone { get; set; }

        public AddressRequest Address { get; set; }

        public List<PhoneRequest> Phones { get; set; }

        public PersonRequest()
        {
            Document = string.Empty;
            Phones = new List<PhoneRequest>();
        }

        public object Clone()
        {
            return new PersonRequest
            {
                Address = (AddressRequest)Address.Clone(),
                BirthDate = BirthDate,
                Document = Document,
                Email = Email,
                Gender = Gender,
                Name = Name,
                Phones = Phones.Select(p => (PhoneRequest)p.Clone()).ToList(),
                MainPhone = (PhoneRequest)MainPhone.Clone(),
            };
        }
    }
}