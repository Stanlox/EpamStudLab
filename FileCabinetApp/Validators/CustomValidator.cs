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
    public class CustomValidator : CompositiveValidator
    {
        public CustomValidator()
            : base(new IRecordValidator[]
            {
            new FirstNameValidator(2, 15),
            new LastNameValidator(2, 15),
            new GenderValidator(),
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Today),
            new SalaryValidator(int.MaxValue),
            new AgeValidator(0, 100),
            })
        {
        }
    }
}
