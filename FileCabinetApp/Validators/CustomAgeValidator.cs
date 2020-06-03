using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomAgeValidator : IRecordValidator
    {
        /// <summary>
        /// checks age for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name=" parameters"/> less than 0. </exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.Age < 0)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(parameters)}");
            }
        }
    }
}
