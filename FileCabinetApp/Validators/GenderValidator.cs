using System;
using System.Linq;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Gender validator.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private char[] gender;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="gender">Input array of the gender.</param>
        public GenderValidator(char[] gender)
        {
            this.gender = gender;
        }

        /// <summary>
        /// checks gender.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown, when gender not defined.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (!this.gender.Contains(parameters.Gender))
            {
                throw new ArgumentException("There is no such gender", $"{nameof(parameters.Gender)}");
            }
        }
    }
}
