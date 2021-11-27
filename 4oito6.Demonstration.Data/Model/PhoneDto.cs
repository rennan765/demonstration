using Dapper.Contrib.Extensions;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_phone")]
    public class PhoneDto
    {
        [Key]
        public int Id { get; set; }

        public int Type { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
    }
}