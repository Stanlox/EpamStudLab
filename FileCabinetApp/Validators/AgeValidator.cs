using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Age validator.
    /// </summary>
    public class AgeValidator : IRecordValidator
    {
        private int min;
        private int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgeValidator"/> class.
        /// </summary>
        /// <param name="min">Input min age.</param>
        /// <param name="max">Input max age.</param>
        public AgeValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Checks age for a negative value.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="parameters.Age"/> less than 0. </exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Age < this.min || parameters.Age > this.max)
            {
                throw new ArgumentException("Invalid Age", $"{nameof(parameters.Age)}");
            }
        }
    }
}
