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

        [Fact(DisplayName = "Validating Equals method.")]
        public void Equals_Success()
        {
            //arrange:
            var id = 1;
            var type = PhoneType.Home;

            var code = "21";
            var number = "27172770";

            var first = new Phone(id, type, code, number);
            var second = new Phone(id, type, code, number);

            //act and assert:
            first.Equals(second).Should().BeTrue();
        }

        [Fact(DisplayName = "Validating Clone method.")]
        public void Clone_Success()
        {
            //arrange:
            var id = 1;
            var type = PhoneType.Home;

            var code = "21";
            var number = "27172770";

            //act:
            var expectedResult = new Phone(id, type, code, number);
            var result = new Phone(id, type, code, number);

            //act and assert:
            expectedResult.Equals(result).Should().BeTrue();
        }

        [Fact(DisplayName = "Validating Match method.")]
        public void Match_Success()
        {
            //arrange:
            var type = PhoneType.Home;

            var code = "21";
            var number = "27172770";

            //act:
            var expectedResult = new Phone(id: 50, type, code, number);
            var result = new Phone(type, code, number);

            //act and assert:
            expectedResult.Match(result).Should().BeTrue();
        }
    }
}