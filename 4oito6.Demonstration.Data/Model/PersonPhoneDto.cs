using Dapper.Contrib.Extensions;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_person_phone")]
    public class PersonPhoneDto
    {
        [Key]
        public long Id { get; set; }

        public int PersonId { get; set; }
        public int PhoneId { get; set; }
    }
}