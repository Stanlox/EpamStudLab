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
        private const string Default = "Default";
        private const string Custom = "Custom";

        /// <summary>
        /// Default extension method.
        /// </summary>
        /// <param name="builder">Input builder.</param>
        /// <returns>new ValidatorBuilder.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder builder) => CreateValidator(Default);

        /// <summary>
        /// Custom extension method.
        /// </summary>
        /// <param name="builder">Input builder.</param>
        /// <returns>new ValidatorBuilder.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder builder) => CreateValidator(Custom);

        private static IRecordValidator CreateValidator(string type)
        {
            var configReader = new ConfigurationReader(type);
            (var firstNameMin, var firstNameMax) = configReader.ReadFirstNameCriterions();
            (var lastNameMin, var lastNameMax) = configReader.ReadLastNameCriterions();
            (var from, var to) = configReader.ReadDateOfBirthCriterions();
            (var ageMin, var ageMax) = configReader.ReadAgeCriterions();
            var salaryMax = configReader.ReadSalaryCriterions();
            var genderRange = configReader.ReadGenderCriterions();

            return new ValidatorBuilder()
               .ValidateFirstName(firstNameMin, firstNameMax)
               .ValidateLastName(lastNameMin, lastNameMax)
               .ValidateDateOfBirth(from, to)
               .ValidateAge(ageMin, ageMax)
               .ValidateSalary(salaryMax)
               .ValidateGender(genderRange)
               .Create();
        }
    }
}
