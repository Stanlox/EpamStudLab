using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads data from a json validation file.
    /// </summary>
    public class ConfigurationReader
    {
        private IConfiguration config;
        private string validType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReader"/> class.
        /// build congiguration json file.
        /// </summary>
        /// <param name="validType">Input type of validation.</param>
        public ConfigurationReader(string validType)
        {
            this.validType = validType;
            var builder = new ConfigurationBuilder()
             .SetBasePath("D:/EpamStudLab/FileCabinetApp/Properties/")
             .AddJsonFile("validation-rules.json");
            this.config = builder.Build();
        }

        /// <summary>
        /// Read first name criterions from json file.
        /// </summary>
        /// <returns>tuple of the minLength and maxLength of the first name.</returns>
        public Tuple<int, int> ReadFirstNameCriterions()
        {
            var firstNameMinLength = this.config.GetSection(this.validType).GetSection("firstName").GetValue<int>("min");
            var firstNameMaxLength = this.config.GetSection(this.validType).GetSection("firstName").GetValue<int>("max");

            return Tuple.Create(firstNameMinLength, firstNameMaxLength);
        }

        /// <summary>
        /// Read last name criterions from json file.
        /// </summary>
        /// <returns>tuple of the minLength and maxLength of the last name.</returns>
        public Tuple<int, int> ReadLastNameCriterions()
        {
            var lastNameMinLength = this.config.GetSection(this.validType).GetSection("lastName").GetValue<int>("min");
            var lastNameMaxLength = this.config.GetSection(this.validType).GetSection("lastName").GetValue<int>("max");

            return Tuple.Create(lastNameMinLength, lastNameMaxLength);
        }

        /// <summary>
        /// Read date of birth criterions from json file.
        /// </summary>
        /// <returns>tuple of the minDate and maxDate of the date of birth.</returns>
        public Tuple<DateTime, DateTime> ReadDateOfBirthCriterions()
        {
            var minDateOfBirth = this.config.GetSection(this.validType).GetSection("dateOfBirth").GetValue<DateTime>("from");
            var maxDateOfBirth = this.config.GetSection(this.validType).GetSection("dateOfBirth").GetValue<DateTime>("to");

            return Tuple.Create(minDateOfBirth, maxDateOfBirth);
        }

        /// <summary>
        /// Read age criterions from json file.
        /// </summary>
        /// <returns>tuple of the minAge and maxAge of the Age.</returns>
        public Tuple<int, int> ReadAgeCriterions()
        {
            var minAge = this.config.GetSection(this.validType).GetSection("age").GetValue<int>("min");
            var maxAge = this.config.GetSection(this.validType).GetSection("age").GetValue<int>("max");

            return Tuple.Create(minAge, maxAge);
        }

        /// <summary>
        /// Read salary criterions from json file.
        /// </summary>
        /// <returns>max salary.</returns>
        public int ReadSalaryCriterions()
        {
            var maxSalary = this.config.GetSection(this.validType).GetSection("salary").GetValue<int>("max");

            return maxSalary;
        }

        /// <summary>
        /// Read gender criterions from json file.
        /// </summary>
        /// <returns>array of acceptable values.</returns>
        public char[] ReadGenderCriterions()
        {
            var rangeOfAcceptableValuesArrayStringFormat = this.config.GetSection(this.validType).GetSection("gender").GetSection("gender").GetChildren().ToArray().Select(c => c.Value).ToArray();
            var rangeOfAcceptableValuesStringFormat = string.Join(string.Empty, rangeOfAcceptableValuesArrayStringFormat);
            var rangeOfAcceptableValues = rangeOfAcceptableValuesStringFormat.ToCharArray();

            return rangeOfAcceptableValues;
        }
    }
}
