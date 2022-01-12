using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_person_phone")]
    [ExcludeFromCodeCoverage]
    public class PersonPhoneDto
    {
        [Key]
        public long personphoneid { get; set; }

        public int personid { get; set; }
        public int phoneid { get; set; }
    }
}