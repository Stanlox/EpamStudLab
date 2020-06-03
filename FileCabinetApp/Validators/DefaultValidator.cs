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
    public class DefaultValidator : IRecordValidator
    {

        /// <summary>
        /// implementation of the method for checking the correctness of user input.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            new FirstNameValidator(2, 60).ValidateParameters(parameters);
            new LastNameValidator(2, 60).ValidateParameters(parameters);
            new GenderValidator().ValidateParameters(parameters);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParameters(parameters);
            new SalaryValidator(int.MaxValue).ValidateParameters(parameters);
            new AgeValidator(0, 130).ValidateParameters(parameters);
        }
    }
}
