using System;

namespace _4oito6.Demonstration.Commons.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsAgeRange(this DateTime date, int minAge, int maxAge)
        {
            var currentDate = DateTime.UtcNow.Date;
            var sumForBissextYear = (int)Math.Floor((double)(currentDate.Year - date.Year) / 4);

            var totalDaysInterval = (DateTime.UtcNow.Date - date).TotalDays + sumForBissextYear;
            var currentAge = Math.Floor(totalDaysInterval / 365);

            return currentAge >= minAge && currentAge <= maxAge;
        }
    }
}