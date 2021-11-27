using Dapper.Contrib.Extensions;
using System;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_person")]
    public class PersonDto
    {
        [Key]
        public int Id { get; private set; }

        public string Name { get; set; }
        public string Document { get; set; }
        public int Gender { get; set; }
        public int? AddressId { get; set; }
        public int MainPhoneId { get; set; }
        public DateTime BirthDate { get; set; }
    }
}