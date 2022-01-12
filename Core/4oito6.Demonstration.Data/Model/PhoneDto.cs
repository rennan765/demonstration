using Dapper.Contrib.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model
{
    [Table("tb_phone")]
    [ExcludeFromCodeCoverage]
    public class PhoneDto : ICloneable
    {
        [Key]
        public int phoneid { get; set; }

        public int type { get; set; }
        public string code { get; set; }
        public string number { get; set; }

        public object Clone()
        {
            return new PhoneDto
            {
                code = code,
                number = number,
                phoneid = phoneid,
                type = type
            };
        }
    }
}