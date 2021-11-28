using Dapper.Contrib.Extensions;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_phone")]
    public class PhoneDto
    {
        [Key]
        public int phoneid { get; set; }

        public int type { get; set; }
        public string code { get; set; }
        public string number { get; set; }
    }
}