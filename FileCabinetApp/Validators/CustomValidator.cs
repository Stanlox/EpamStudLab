using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains methods for checking data input user.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// implementation of the method for checking the correctness of user input.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            new CustomFirstNameValidator().ValidateParameters(parameters);
            new CustomLastNameValidator().ValidateParameters(parameters);
            new CustomGenderValidator().ValidateParameters(parameters);
            new CustomDateOfBirthValidator().ValidateParameters(parameters);
            new CustomSalaryValidator().ValidateParameters(parameters);
            new CustomAgeValidator().ValidateParameters(parameters);
        }
    }
}
