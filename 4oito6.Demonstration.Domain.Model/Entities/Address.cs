using System;

namespace _4oito6.Demonstration.Domain.Model.Entities
{
    public class Address : ICloneable
    {
        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="street"></param>
        /// <param name="number"></param>
        /// <param name="complement"></param>
        /// <param name="district"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        public Address(string street, string number, string complement, string district, string city, string state)
        {
            Street = street;
            Number = number;
            Complement = complement;
            District = district;
            City = city;
            State = state;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="street"></param>
        /// <param name="number"></param>
        /// <param name="complement"></param>
        /// <param name="district"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        public Address(int id, string street, string number, string complement, string district, string city, string state)
            : this(street, number, complement, district, city, state)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string Complement { get; private set; }
        public string District { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }

        public object Clone()
        {
            return new Address(Id, Street, Number, Complement, District, City, State);
        }
    }
}