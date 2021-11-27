using System;

namespace _4oito6.Demonstration.Commons.Extensions
{
    public static class StringExtension
    {
        private static bool IsFirstDigitValid(this string document)
        {
            var lastDigits = document.Substring(document.Length - 2);

            var digitSum = Convert.ToInt32(document.Substring(1, 1)) * 10 +
                Convert.ToInt32(document.Substring(1, 1)) * 9 +
                Convert.ToInt32(document.Substring(3, 1)) * 8 +
                Convert.ToInt32(document.Substring(4, 1)) * 7 +
                Convert.ToInt32(document.Substring(5, 1)) * 6 +
                Convert.ToInt32(document.Substring(6, 1)) * 5 +
                Convert.ToInt32(document.Substring(7, 1)) * 4 +
                Convert.ToInt32(document.Substring(8, 1)) * 3 +
                Convert.ToInt32(document.Substring(9, 1)) * 2;

            var module = digitSum * 10 % 11;
            return lastDigits.StartsWith((module == 10 ? 0 : module).ToString());
        }

        private static bool IsSecondDigitValid(this string document)
        {
            var lastDigits = document.Substring(document.Length - 2);

            var digitSum = Convert.ToInt32(document.Substring(1, 1)) * 11 +
                Convert.ToInt32(document.Substring(2, 1)) * 10 +
                Convert.ToInt32(document.Substring(3, 1)) * 9 +
                Convert.ToInt32(document.Substring(4, 1)) * 8 +
                Convert.ToInt32(document.Substring(5, 1)) * 7 +
                Convert.ToInt32(document.Substring(6, 1)) * 6 +
                Convert.ToInt32(document.Substring(7, 1)) * 5 +
                Convert.ToInt32(document.Substring(8, 1)) * 4 +
                Convert.ToInt32(document.Substring(9, 1)) * 3 +
                Convert.ToInt32(document.Substring(10, 1)) * 2;

            var module = digitSum * 10 % 11;
            return lastDigits.EndsWith((module == 10 ? 0 : module).ToString());
        }

        public static bool IsDocumentValid(this string document)
        {
            if (string.IsNullOrEmpty(document))
            {
                return false;
            }

            if (long.TryParse(document, out var result))
            {
                return false;
            }

            return document.IsFirstDigitValid() && document.IsSecondDigitValid();
        }
    }
}