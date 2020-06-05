using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Salary validator.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="max">Input max salary.</param>
        public SalaryValidator(int max)
        {
            this.max = max;
        }

        /// <summary>
        /// Checks salary for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown, when user entered a negative amount.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.Salary > this.max)
            {
                throw new ArgumentException("You entered too much salary", $"{nameof(parameters.Salary)}");
            }
        }
    }
}
