using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private int maxLength;
        private int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimal length of the first name.</param>
        /// <param name="maxLength">Maximal length of the first name.</param>
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
