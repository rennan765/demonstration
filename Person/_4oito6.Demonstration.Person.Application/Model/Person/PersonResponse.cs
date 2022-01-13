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

        public bool Match(PersonResponse obj)
        {
            //validating address
            if (!(Address is null && obj.Address is null) && !Address.Match(obj.Address))
            {
                return false;
            }

            //validating phones
            if (obj.Phones.Count != Phones.Count)
            {
                return false;
            }

            var thisPhones = Phones.ToDictionary(p => $"{(int)p.Type} - ({p.Code}) {p.Number}");
            var personPhones = obj.Phones.ToDictionary(p => $"{(int)p.Type} - ({p.Code}) {p.Number}");

            foreach (var phone in thisPhones)
            {
                if (!personPhones.ContainsKey(phone.Key))
                {
                    return false;
                }

                if (!personPhones[phone.Key].Match(phone.Value))
                {
                    return false;
                }
            }

            if (!MainPhone.Match(obj.MainPhone))
            {
                return false;
            }

            //validate person
            return obj.Id == Id && obj.Name == Name &&
                obj.Document == Document && obj.Email == Email &&
                obj.Gender == Gender && obj.BirthDate == BirthDate;
        }
    }
}