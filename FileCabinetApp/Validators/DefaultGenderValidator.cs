using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultGenderValidator
    {
        private const string CheckMenGender = "M";
        private const string CheckWomenGender = "W";

        /// <summary>
        /// checks gender.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown, when gender not defined.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            string stringGender = parameters.Gender.ToString(CultureInfo.CurrentCulture);
            if (!(stringGender.Equals(CheckMenGender, StringComparison.InvariantCultureIgnoreCase) || stringGender.Equals(CheckWomenGender, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("There is no such gender", $"{nameof(parameters.Gender)}");
            }
        }
    }
}
