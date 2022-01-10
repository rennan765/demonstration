using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model
{
    [ExcludeFromCodeCoverage]
    public class CompletePersonDto : PersonDto, ICloneable
    {
        public PhoneDto Phone { get; set; }
        public AddressDto Address { get; set; }

        public override object Clone()
        {
            return new CompletePersonDto
            {
                Phone = (PhoneDto)Phone?.Clone(),
                Address = (AddressDto)Address?.Clone(),
                addressid = addressid,
                birthdate = birthdate,
                document = document,
                email = email,
                gender = gender,
                mainphoneid = mainphoneid,
                name = name,
                personid = personid
            };
        }
    }
}