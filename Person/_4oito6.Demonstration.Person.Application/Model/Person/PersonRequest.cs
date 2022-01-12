using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Person.Application.Model.Person
{
    public class PersonRequest : ICloneable
    {
        private readonly List<PhoneRequest> _phones;

        public string Name { get; set; }

        public string Document { get; set; }

        public string Email { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public PhoneRequest MainPhone { get; set; }

        public AddressRequest Address { get; set; }

        public IEnumerable<PhoneRequest> Phones
        {
            get => _phones;
            set
            {
                _phones.AddRange(value ?? new PhoneRequest[0]);
            }
        }

        public PersonRequest()
        {
            Document = string.Empty;
            _phones = new List<PhoneRequest>();
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
                Phones = _phones.Select(p => (PhoneRequest)p.Clone()),
                MainPhone = (PhoneRequest)MainPhone.Clone(),
            };
        }
    }
}