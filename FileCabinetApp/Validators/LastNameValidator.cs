using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private int maxLength;
        private int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimal length of the last name.</param>
        /// <param name="maxLength">Maximal length of the last name.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.maxLength = maxLength;
            this.minLength = minLength;
        }

        /// <summary>
        /// Checks the string length.
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
