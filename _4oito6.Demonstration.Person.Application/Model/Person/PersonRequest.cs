using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Person.Application.Model.Address;
using _4oito6.Demonstration.Person.Application.Model.Phone;
using System;
using System.Collections.Generic;

namespace _4oito6.Demonstration.Person.Application.Model.Person
{
    public class PersonRequest
    {
        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public PhoneRequest MainPhone { get; set; }
        public List<PhoneRequest> Phones { get; private set; }
        public AddressRequest Address { get; set; }

        public PersonRequest()
        {
            Phones = new List<PhoneRequest>();
        }

        public PersonRequest(string name, string document, string email, Gender gender, DateTime birthDate, PhoneRequest mainPhone, List<PhoneRequest> phones, AddressRequest address) : this()
        {
            Name = name;
            Document = document;
            Email = email;
            Gender = gender;
            BirthDate = birthDate;
            MainPhone = mainPhone;
            Phones = phones;
            Address = address;
        }
    }
}