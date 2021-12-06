using _4oito6.Demonstration.Domain.Model.Entities.Base;
using _4oito6.Demonstration.Domain.Model.Enum;
using _4oito6.Demonstration.Domain.Model.Validators;
using System;

namespace _4oito6.Demonstration.Domain.Model.Entities
{
    public class Phone : EntityBase, ICloneable
    {
        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="number"></param>
        public Phone(PhoneType type, string code, string number)
        {
            Type = type;
            Code = code;
            Number = number;
            Validate(this, new CreatePhoneValidator());
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="number"></param>
        public Phone(int id, PhoneType type, string code, string number)
            : this(type, code, number)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public PhoneType Type { get; private set; }
        public string Code { get; private set; }
        public string Number { get; private set; }

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

        public bool Match(Phone phone)
        {
            return ToString().Equals(phone.ToString());
        }
    }
}