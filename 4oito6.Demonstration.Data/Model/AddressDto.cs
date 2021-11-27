using Dapper.Contrib.Extensions;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_address")]
    public class AddressDto
    {
        [Key]
        public int Id { get; set; }

        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}