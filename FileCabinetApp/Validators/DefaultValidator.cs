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
    public class DefaultValidator : CompositiveValidator
    {

        public DefaultValidator()
            : base(new IRecordValidator[]
        {
            new FirstNameValidator(2, 60),
            new LastNameValidator(2, 60),
            new GenderValidator(),
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now),
            new SalaryValidator(int.MaxValue),
            new AgeValidator(0, 130),
        })
        {
        }
    }
}
