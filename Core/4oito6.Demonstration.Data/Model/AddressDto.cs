using Dapper.Contrib.Extensions;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_address")]
    public class AddressDto
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
    }
}