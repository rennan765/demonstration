using _4oito6.Demonstration.Domain.Model.Enum;
using System;

namespace _4oito6.Demonstration.Person.Application.Model.Phone
{
    public class PhoneRequest : ICloneable
    {
        public PhoneType Type { get; set; }

        public string Code { get; set; }

        public string Number { get; set; }

        public object Clone()
        {
            return new PhoneRequest
            {
                Type = Type,
                Code = Code,
                Number = Number
            };
        }
    }
}