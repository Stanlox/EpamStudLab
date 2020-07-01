using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Date of birth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Minimal date of birth.</param>
        /// <param name="to">Maximal date of birth.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// checks the date of birth range.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="parameters.DateOfBirth"/> is the wrong range.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (DateTime.Compare(this.to, parameters.DateOfBirth) < 0 || DateTime.Compare(this.from, parameters.DateOfBirth) > 0)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(parameters.DateOfBirth)}");
            }
        }
    }
}
