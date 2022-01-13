using _4oito6.Demonstration.Commons.Extensions;
using FluentAssertions;
using System;
using System.Globalization;
using Xunit;

namespace _4oito6.Demonstration.Test.Commons.Extensions
{
    [Trait("DateTimeExtensions", "Extension Methods tests")]
    public class DateTimeExtensionsTest
    {
        [Theory(DisplayName = "Testing if age is in specific range.")]
        [InlineData("1991-07-11", 18, 1000, true)]
        [InlineData("1993-08-04", 18, 21, false)]
        public void IsAgeRange_Success(string date, int minAge, int maxAge, bool isResult)
        {
            var parsedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            (isResult == parsedDate.IsAgeRange(minAge, maxAge)).Should().BeTrue();
        }

        [Fact(DisplayName = "Testing if age is under 18.")]
        public void IsAgeRange_SuccessNotUnder18()
        {
            DateTime.UtcNow.Date.IsAgeRange(minAge: 18).Should().BeFalse();
        }
    }
}