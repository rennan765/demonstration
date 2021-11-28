using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System;
using System.Collections.Generic;

namespace _4oito6.Demonstration.Person.Application.Model.Person
{
    public class PersonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public PhoneResponse MainPhone { get; set; }
        public List<PhoneResponse> Phones { get; private set; }
        public AddressResponse? Address { get; set; }

        public PersonResponse()
        {
            Phones = new List<PhoneResponse>();
        }
    }
}