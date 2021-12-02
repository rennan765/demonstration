using _4oito6.Demonstration.Domain.Model.Entities.Base;
using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Domain.Model.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Domain.Model.Entities
{
    public class Person : EntityBase, ICloneable
    {
        private readonly Dictionary<string, Phone> _phones;

        /// <summary>
        /// Private constructor, used to encapsulate main properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        private Person(string name, string email, string document, Gender gender, DateTime birthDate)
        {
            _phones = new Dictionary<string, Phone>();

            Name = name;
            Email = email;
            Document = document.Replace("-", "").Replace(".", "");
            Gender = gender;
            BirthDate = birthDate;
        }

        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        /// <param name="address"></param>
        private Person
        (
            string name,
            string email,
            string document,

            Gender gender,
            DateTime birthDate,
            Address? address = null
        ) : this(name, email, document, gender, birthDate)
        {
            if (address != null)
            {
                Attach(address);
            }
        }

        /// <summary>
        /// Create constructor (with phone info)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        /// <param name="phones"></param>
        /// <param name="mainPhone"></param>
        /// <param name="address"></param>
        public Person
        (
            string name,
            string email,
            string document,

            Gender gender,
            DateTime birthDate,

            IEnumerable<Phone> phones,
            Phone? mainPhone = null,
            Address? address = null
        ) : this(name, email, document, gender, birthDate, address)
        {
            if (phones != null)
            {
                Attach(phones);
            }

            if (mainPhone != null)
            {
                Attach(mainPhone, isMainPhone: true);
            }

            ValidateToCreate();
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="birthDate"></param>
        /// <param name="phones"></param>
        /// <param name="mainPhone"></param>
        /// <param name="address"></param>
        public Person
        (
            int id,
            string name,

            string email,
            string document,

            Gender gender,
            DateTime birthDate,

            IEnumerable<Phone> phones,
            Phone? mainPhone = null,
            Address? address = null
        ) : this(name, email, document, gender, birthDate, phones, mainPhone, address)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Document { get; private set; }
        public Gender Gender { get; private set; }
        public DateTime BirthDate { get; private set; }

        public Address? Address { get; private set; }
        public IEnumerable<Phone> Phones => _phones.Select(p => p.Value);
        public Phone MainPhone { get; private set; }

        public void Attach(Address address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public void RemoveAddress()
        {
            Address = null;
        }

        public void Attach(Phone phone, bool isMainPhone = false)
        {
            if (phone is null)
            {
                throw new ArgumentNullException(nameof(phone));
            }

            if (!_phones.ContainsKey(phone.ToString()))
            {
                _phones.Add(phone.ToString(), phone);
            }

            if (isMainPhone)
            {
                MainPhone = _phones[phone.ToString()];
            }
        }

        public void Attach(IEnumerable<Phone> phones)
        {
            if (phones is null)
            {
                throw new ArgumentNullException(nameof(phones));
            }

            if (!phones.Any())
            {
                return;
            }

            foreach (var phone in phones)
            {
                Attach(phone);
            }
        }

        public bool ValidateToCreate() => Validate(this, new CreatePersonValidator());

        public bool ValidateToUpdate() => Validate(this, new UpdatePersonValidator());

        public void ClearPhones()
        {
            _phones.Clear();
            MainPhone = null;
        }

        public void ClearAddress()
        {
            _phones.Clear();
            MainPhone = null;
        }

        public static Person GetDefaultInstance()
        {
            return new Person
            (
                name: string.Empty,
                email: string.Empty,
                document: "00000000000",

                gender: Gender.NotInformed,
                birthDate: DateTime.UtcNow
            );
        }

        public object Clone()
        {
            return new Person
            (
                id: Id,
                name: Name,
                email: Email,

                document: Document,
                gender: Gender,
                birthDate: BirthDate,

                phones: Phones.Select(p => (Phone)p.Clone()),
                mainPhone: (Phone)MainPhone.Clone(),
                address: (Address?)Address?.Clone()
            );
        }
    }
}