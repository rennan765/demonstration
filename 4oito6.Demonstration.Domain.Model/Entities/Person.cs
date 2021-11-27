﻿using _4oito6.Demonstration.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Domain.Model.Entities
{
    public class Person
    {
        private readonly Dictionary<string, Phone> _phones;

        /// <summary>
        /// Private constructor, used to encapsulate main properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        private Person(string name, string document, Gender gender)
        {
            _phones = new Dictionary<string, Phone>();

            Name = name;
            Document = document;
            Gender = gender;
        }

        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="address"></param>
        public Person
        (
            string name,
            string document,
            Gender gender,
            Address? address = null
        ) : this(name, document, gender)
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
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="phones"></param>
        /// <param name="mainPhone"></param>
        /// <param name="address"></param>
        public Person
        (
            string name,
            string document,
            Gender gender,

            IEnumerable<Phone> phones,
            Phone? mainPhone = null,
            Address? address = null
        ) : this(name, document, gender, address)
        {
            if (phones != null)
            {
                Attach(phones);
            }

            if (mainPhone != null)
            {
                Attach(mainPhone, isMainPhone: true);
            }
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="document"></param>
        /// <param name="gender"></param>
        /// <param name="phones"></param>
        /// <param name="mainPhone"></param>
        /// <param name="address"></param>
        public Person
        (
            int id,
            string name,
            string document,
            Gender gender,

            IEnumerable<Phone> phones,
            Phone? mainPhone = null,
            Address? address = null
        ) : this(name, document, gender, phones, mainPhone, address)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Document { get; private set; }
        public Gender Gender { get; private set; }

        public Address? Address { get; private set; }
        public IEnumerable<Phone> Phones => _phones.Select(p => p.Value);
        public Phone? MainPhone { get; private set; }

        public void Attach(Address address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public void Attach(Phone phone, bool isMainPhone = false)
        {
            if (phone is null)
            {
                throw new ArgumentNullException(nameof(phone));
            }

            if (_phones.ContainsKey(phone.ToString()))
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
    }
}