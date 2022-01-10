using Dapper.Contrib.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_address")]
    [ExcludeFromCodeCoverage]
    public class AddressDto : ICloneable
    {
        [Key]
        public int addressid { get; set; }

        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }

        public object Clone()
        {
            return new AddressDto
            {
                addressid = addressid,
                street = street,
                number = number,
                complement = complement,
                city = city,
                state = state,
                postalcode = postalcode,
                district = district
            };
        }
    }
}