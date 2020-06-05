using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Contains extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Default extension method.
        /// </summary>
        /// <param name="builder">Input builder.</param>
        /// <returns>new ValidatorBuilder.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder builder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today)
                .ValidateAge(0, 130)
                .ValidateSalary(int.MaxValue)
                .ValidateGender(new char[] { 'W', 'w', 'M', 'm' })
                .Create();
        }

        /// <summary>
        /// Custom extension method.
        /// </summary>
        /// <param name="builder">Input builder.</param>
        /// <returns>new ValidatorBuilder.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder builder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 15)
                .ValidateLastName(2, 15)
                .ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Today)
                .ValidateAge(0, 100)
                .ValidateSalary(int.MaxValue)
                .ValidateGender(new char[] { 'W', 'w', 'M', 'm' })
                .Create();
        }
    }
}
