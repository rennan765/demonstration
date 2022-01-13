using _4oito6.Demonstration.Domain.Model.Enum;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.Application.Model.Phone
{
    [ExcludeFromCodeCoverage]
    public class PhoneResponse
    {
        public int Id { get; set; }
        public PhoneType Type { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }

        public bool Match(PhoneResponse obj)
        {
            return obj.Id == Id && obj.Type == Type && obj.Code == Code && obj.Number == Number;
        }
    }
}