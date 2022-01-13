using _4oito6.Demonstration.Domain.Model.Entities;
using FluentAssertions;
using Xunit;

namespace _4oito6.Demonstration.Test.Domain.Model.Entities
{
    [Trait("Address", "Domain Model tests")]
    public class AddressTest
    {
        [Fact(DisplayName = "Attempting do create a new Address with invalid entries.")]
        public void Validate_Failure()
        {
            //arrange:
            var address = new Address
            (
                street: string.Empty,

                number: string.Empty,
                complement: string.Empty,

                district: string.Empty,
                city: string.Empty,

                state: string.Empty,
                postalCode: "\"not a number\" and too big information!"
            );

            //act and assert:
            address.IsValid.Should().BeFalse();
        }

        [Fact(DisplayName = "Address created successfully.")]
        public void Validate_Success()
        {
            //arrange:
            var address = new Address
            (
                street: "Avenida Rio Branco",

                number: "156",
                complement: null,

                district: "Centro",
                city: "Rio de Janeiro",

                state: "RJ",
                postalCode: "20040-901"
            );

            //act and assert:
            address.IsValid.Should().BeTrue();
        }
    }
}