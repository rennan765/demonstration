using _4oito6.Demonstration.Domain.Model.Enum;

namespace _4oito6.Demonstration.Person.Application.Model.Phone
{
    public class PhoneRequest
    {
        public PhoneType Type { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
    }
}