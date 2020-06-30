using System;
using System.Collections.Generic;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Class implements a pattern fluetn builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Creates first name validator.
        /// </summary>
        /// <param name="minLength">Minimal length of the name.</param>
        /// <param name="maxLength">Maximal length of the name.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Creates last name validator.
        /// </summary>
        /// <param name="minLength">Minimal length of the name.</param>
        /// <param name="maxLength">Maximal length of the name.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Creates date of birth validator.
        /// </summary>
        /// <param name="datetimeFrom">Minimal date of birth.</param>
        /// <param name="datetimeTo">Maximal date of birth.</param>
        /// <returns>Current buillder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime datetimeFrom, DateTime datetimeTo)
        {
            this.validators.Add(new DateOfBirthValidator(datetimeFrom, datetimeTo));
            return this;
        }

        /// <summary>
        /// Creates gender validator.
        /// </summary>
        /// <param name="gender">Array of the gender.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateGender(char[] gender)
        {
            this.validators.Add(new GenderValidator(gender));
            return this;
        }

        /// <summary>
        /// Creates salary validator.
        /// </summary>
        /// <param name="maxSalary">Maximal salary.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateSalary(int maxSalary)
        {
            this.validators.Add(new SalaryValidator(maxSalary));
            return this;
        }

        /// <summary>
        /// Creates age validator.
        /// </summary>
        /// <param name="minAge">Minimal age.</param>
        /// <param name="maxAge">Maximum age.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateAge(int minAge, int maxAge)
        {
            this.validators.Add(new AgeValidator(minAge, maxAge));
            return this;
        }

        /// <summary>
        /// Returns the created object.
        /// </summary>
        /// <returns>new Compositive validator.</returns>
        public CompositiveValidator Create()
        {
            return new CompositiveValidator(this.validators);
        }
    }
}
