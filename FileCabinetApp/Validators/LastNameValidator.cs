using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class LastNameValidator
    {
        private int maxLength;
        private int minLength;

        public LastNameValidator(int minLength, int maxLength)
        {
            this.maxLength = maxLength;
            this.minLength = minLength;
        }

        /// <summary>
        /// checks the string length.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown when string is not corrct length.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.LastName.Length < this.minLength | parameters.LastName.Length > this.maxLength)
            {
                throw new ArgumentException("Invalid length.", $"{nameof(parameters.LastName)}");
            }
        }
    }
}
