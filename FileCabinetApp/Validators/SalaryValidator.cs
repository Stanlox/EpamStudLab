using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class SalaryValidator : IRecordValidator
    {
        private int max;

        public SalaryValidator(int max)
        {
            this.max = max;
        }

        /// <summary>
        /// checks salary for a negative value.
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
