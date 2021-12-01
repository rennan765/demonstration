namespace _4oito6.Demonstration.Data.Model
{
    public class CompletePersonDto : PersonDto
    {
        public PhoneDto Phone { get; set; }
        public AddressDto Address { get; set; }
    }
}