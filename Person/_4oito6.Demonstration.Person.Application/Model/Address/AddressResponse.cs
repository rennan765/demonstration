using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.Application.Model.Address
{
    [ExcludeFromCodeCoverage]
    public class AddressResponse
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public bool Match(AddressResponse obj)
        {
            return obj.Id == Id && obj.Street == Street &&
                obj.Number == Number && obj.City == City &&
                obj.Complement == Complement && obj.District == District &&
                obj.State == State && obj.PostalCode == PostalCode;
        }
    }
}