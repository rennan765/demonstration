using Dapper.Contrib.Extensions;
using System;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_person")]
    public class PersonDto
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string document { get; set; }
        public string email { get; set; }
        public int gender { get; set; }
        public int? addressid { get; set; }
        public int mainphoneid { get; set; }
        public DateTime birthdate { get; set; }
    }
}