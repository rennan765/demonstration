using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System;
using System.Collections.Generic;

namespace _4oito6.Demonstration.Person.Application.Model.Person
{
    public class PersonRequest
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
            _phones = new List<PhoneRequest>();
        }
    }
}