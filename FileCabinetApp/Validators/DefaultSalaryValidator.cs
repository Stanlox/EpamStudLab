using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultSalaryValidator
    {
        /// <summary>
        /// checks salary for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown, when user entered a negative amount.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.Salary > int.MaxValue)
            {
                throw new ArgumentException("You entered too much salary", $"{nameof(parameters.Salary)}");
            }
        }
    }
}
