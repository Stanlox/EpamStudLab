using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class FirstNameValidator
    {
        private int maxLength;
        private int minLength;

        public FirstNameValidator(int minLength, int maxLength)
        {
            this.maxLength = maxLength;
            this.minLength = minLength;
        }

        /// <summary>
        /// checks the string for null and empty.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown when string is not correct length.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.FirstName.Length < this.minLength | parameters.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException("Invalid length.", $"{nameof(parameters.FirstName)}");
            }
        }
    }
}
