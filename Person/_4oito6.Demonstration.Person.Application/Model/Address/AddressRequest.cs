using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.Application.Model.Address
{
    public class AddressRequest : ICloneable
    {
        public string Street { get; set; }

        public string Number { get; set; }

        public string Complement { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        [ExcludeFromCodeCoverage]
        public object Clone()
        {
            return new AddressRequest
            {
                City = City,
                State = State,
                PostalCode = PostalCode,
                Complement = Complement,
                District = District,
                Street = Street,
                Number = Number
            };
        }
    }
}