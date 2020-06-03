using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultAgeValidator
    {
        /// <summary>
        /// checks age for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="parameters.Age"/> less than 0. </exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.Age < 0 || parameters.Age > 130)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(parameters.Age)}");
            }
        }
    }
}
