using _4oito6.Demonstration.Domain.Model.Enum;
using System;

namespace _4oito6.Demonstration.Domain.Model.Entities
{
    public class Phone : ICloneable
    {
        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="number"></param>
        public Phone(PhoneType type, int code, int number)
        {
            Type = type;
            Code = code;
            Number = number;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="number"></param>
        public Phone(int id, PhoneType type, int code, int number)
            : this(type, code, number)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public PhoneType Type { get; private set; }
        public int Code { get; private set; }
        public int Number { get; private set; }

        public override string ToString() => $"{(int)Type} - ({Code}) {Number}";

        public override bool Equals(object obj)
        {
            var phone = (Phone)obj;
            return phone.Id == Id && phone.Type == Type && phone.Code == Code && phone.Number == Number;
        }

        public object Clone()
        {
            return new Phone(Id, Type, Code, Number);
        }
    }
}