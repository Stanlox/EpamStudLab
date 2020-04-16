using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public static class GuardClauses
    {
        private const string CheckMenGender = "M";
        private const string CheckWomenGender = "W";
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);

        public static void IsNullOrEmpty(string firstNameValue, string firstName, string lastNameValue, string lastName)
        {
            if (string.IsNullOrEmpty(firstNameValue))
            {
                throw new ArgumentNullException(nameof(firstName));
            }
            else if (string.IsNullOrEmpty(lastNameValue))
            {
                throw new ArgumentNullException(nameof(lastName));
            }
        }

        public static void CheckLength(string firstNameValue, string firstName, string lastNameValue, string lastName)
        {
            #pragma warning disable CA1062 // Проверить аргументы или открытые методы
            if (firstNameValue.Length < 2 | firstNameValue.Length > 60)
            #pragma warning restore CA1062 // Проверить аргументы или открытые методы
            {
                throw new ArgumentException("Invalid length.", $"{nameof(firstName)}");
            }
            #pragma warning disable CA1062 // Проверить аргументы или открытые методы
            else if (lastNameValue.Length < 2 | lastNameValue.Length > 60)
            #pragma warning restore CA1062 // Проверить аргументы или открытые методы
            {
                throw new ArgumentException("Invalid length.", $"{nameof(lastName)}");
            }
        }

        public static void CheckDateRange(DateTime dateOfBirthValue, string dateOfBirth)
        {
            if (DateTime.Compare(DateTime.Now, dateOfBirthValue) < 0 || DateTime.Compare(MinDate, dateOfBirthValue) > 0)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(dateOfBirth)}");
            }
        }

        public static void CheckAge(short ageValue, string age)
        {
            if (ageValue < 0)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(age)}");
            }
        }

        public static void CheckGender(char genderValue, string gender)
        {
            string stringGender = genderValue.ToString(CultureInfo.CurrentCulture);
            if (!(stringGender.Equals(CheckMenGender, StringComparison.InvariantCultureIgnoreCase) || stringGender.Equals(CheckWomenGender, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("There is no such gender", $"{nameof(gender)}");
            }
        }

        public static void CheckSalarySign(decimal salaryValue, string salary)
        {
            if (salaryValue < 0)
            {
                throw new ArgumentException("You entered a negative amount", $"{nameof(salary)}");
            }
        }
    }
}
