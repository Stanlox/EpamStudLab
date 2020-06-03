using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DateOfBirthValidator
    {
        private DateTime from;
        private DateTime to;

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
            if (DateTime.Compare(this.to, parameters.DateOfBirth) < 0 || DateTime.Compare(this.from, parameters.DateOfBirth) > 0)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(parameters.DateOfBirth)}");
            }
        }
    }
}
