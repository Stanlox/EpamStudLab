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
            new DefaultFirstNameValidator().ValidateParameters(parameters);
            new DefaultLastNameValidator().ValidateParameters(parameters);
            new DefaultGenderValidator().ValidateParameters(parameters);
            new DefaultDateOfBirthValidator().ValidateParameters(parameters);
            new DefaultSalaryValidator().ValidateParameters(parameters);
            new DefaultAgeValidator().ValidateParameters(parameters);
        }
    }
}
