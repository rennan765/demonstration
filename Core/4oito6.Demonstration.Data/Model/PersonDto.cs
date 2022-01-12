using Dapper.Contrib.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_person")]
    [ExcludeFromCodeCoverage]
    public class PersonDto : ICloneable
    {
        [Key]
        public int personid { get; set; }

        public string name { get; set; }
        public string document { get; set; }
        public string email { get; set; }
        public int gender { get; set; }
        public int? addressid { get; set; }
        public int mainphoneid { get; set; }
        public DateTime birthdate { get; set; }

        public virtual object Clone()
        {
            return new PersonDto
            {
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