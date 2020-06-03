using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class AgeValidator : IRecordValidator
    {
        private int min;
        private int max;

        public AgeValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// checks age for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="parameters.Age"/> less than 0. </exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.Age < this.min || parameters.Age > this.max)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(parameters.Age)}");
            }
        }
    }
}
