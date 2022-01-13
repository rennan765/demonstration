using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using FluentAssertions;
using Xunit;

namespace _4oito6.Demonstration.Test.Domain.Model.Entities
{
    [Trait("Phone", "Domain Model tests")]
    public class PhoneTest
    {
        [Fact(DisplayName = "Attempting do create a new Phone with invalid entries.")]
        public void Validate_Failure()
        {
            var phone = new Phone(PhoneType.Home, string.Empty, string.Empty);
            phone.IsValid.Should().BeFalse();
        }

        [Fact(DisplayName = "Phone created successfully.")]
        public void Validate_Success()
        {
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            phone.IsValid.Should().BeTrue();
        }

        [Fact(DisplayName = "Validating ToString method.")]
        public void ToString_Success()
        {
            //arrange:
            var type = PhoneType.Home;
            var code = "21";
            var number = "27172770";

            //act:
            var expectedResult = $"{(int)type} - ({code}) {number}";
            var result = new Phone(id: 1, type, code, number).ToString();

            //assert:
            expectedResult.Equals(result).Should().BeTrue();
        }
    }
}