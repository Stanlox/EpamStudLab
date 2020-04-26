using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains methods for checking data input user.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private const string CheckMenGender = "M";
        private const string CheckWomenGender = "W";
        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// implementation of the method for checking the correctness of user input.
        /// </summary>
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void CheckUsersDataEntry(FileCabinetServiceContext objectParameter)
        {
            IsNullOrEmpty(objectParameter.FirstName, nameof(objectParameter.FirstName), objectParameter.LastName, nameof(objectParameter.LastName));
            CheckLength(objectParameter.FirstName, nameof(objectParameter.FirstName), objectParameter.LastName, nameof(objectParameter.LastName));
            CheckDateRange(objectParameter.DateOfBirth, nameof(objectParameter.DateOfBirth));
            CheckGender(objectParameter.Gender, nameof(objectParameter.Gender));
            CheckSalarySign(objectParameter.Salary, nameof(objectParameter.Salary));
            CheckAge(objectParameter.Age, nameof(objectParameter.Age));
        }

        /// <summary>
        /// checks the string for null and empty.
        /// </summary>
        /// <param name="firstNameValue">variable value firstName.</param>
        /// <param name="firstName">variable name firstName.</param>
        /// <param name="lastNameValue">variable value lastName.</param>
        /// <param name="lastName">variable name lastName.</param>
        /// <exception cref="ArgumentNullException">thrown when string is null or empty.</exception>
        protected static void IsNullOrEmpty(string firstNameValue, string firstName, string lastNameValue, string lastName)
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

        /// <summary>
        /// checks the string length.
        /// </summary>
        /// <param name="firstNameValue">variable value firstName.</param>
        /// <param name="firstName">variable name firstName.</param>
        /// <param name="lastNameValue">variable value lastName.</param>
        /// <param name="lastName">variable name lastName.</param>
        /// <exception cref="ArgumentException">Thrown when string is not corrct length.</exception>
        protected static void CheckLength(string firstNameValue, string firstName, string lastNameValue, string lastName)
        {
            #pragma warning disable CA1062 // Проверить аргументы или открытые методы
            if (firstNameValue.Length < 2 | firstNameValue.Length > 15)
            #pragma warning restore CA1062 // Проверить аргументы или открытые методы
            {
                throw new ArgumentException("Invalid length.", $"{nameof(firstName)}");
            }
            #pragma warning disable CA1062 // Проверить аргументы или открытые методы
            else if (lastNameValue.Length < 2 | lastNameValue.Length > 15)
            #pragma warning restore CA1062 // Проверить аргументы или открытые методы
            {
                throw new ArgumentException("Invalid length.", $"{nameof(lastName)}");
            }
        }

        /// <summary>
        /// checks the date of birth range.
        /// </summary>
        /// <param name="dateOfBirthValue">variable value dateOfBirth.</param>
        /// <param name="dateOfBirth">variable name lastName.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="dateOfBirthValue"/> is the wrong range.</exception>
        protected static void CheckDateRange(DateTime dateOfBirthValue, string dateOfBirth)
        {
            if (DateTime.Compare(DateTime.Today, dateOfBirthValue) < 0 || DateTime.Compare(MinDate, dateOfBirthValue) > 0)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(dateOfBirth)}");
            }
        }

        /// <summary>
        /// checks age for a negative value.
        /// </summary>
        /// <param name="ageValue">variable value age.</param>
        /// <param name="age">variable name age.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="ageValue"/> less than 0. </exception>
        protected static void CheckAge(short ageValue, string age)
        {
            if (ageValue < 0)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(age)}");
            }
        }

        /// <summary>
        /// checks gender.
        /// </summary>
        /// <param name="genderValue">variable value gender.</param>
        /// <param name="gender">variable name gender.</param>
        /// <exception cref="ArgumentException">thrown, when gender not defined.</exception>
        protected static void CheckGender(char genderValue, string gender)
        {
            string stringGender = genderValue.ToString(CultureInfo.CurrentCulture);
            if (!(stringGender.Equals(CheckMenGender, StringComparison.InvariantCultureIgnoreCase) || stringGender.Equals(CheckWomenGender, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("There is no such gender", $"{nameof(gender)}");
            }
        }

        /// <summary>
        /// checks salary for a negative value.
        /// </summary>
        /// <param name="salaryValue">variable value salary.</param>
        /// <param name="salary">variable name.</param>
        /// <exception cref="ArgumentException">thrown, when user entered a negative amount.</exception>
        protected static void CheckSalarySign(decimal salaryValue, string salary)
        {
            if (salaryValue < 0)
            {
                throw new ArgumentException("You entered a negative amount", $"{nameof(salary)}");
            }
        }
    }
}
