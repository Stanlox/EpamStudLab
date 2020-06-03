using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class GenderValidator
    {
        private const string MenGender = "M";
        private const string WomenGender = "W";

        /// <summary>
        /// checks gender.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown, when gender not defined.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            string stringGender = parameters.Gender.ToString(CultureInfo.CurrentCulture);
            if (!(stringGender.Equals(MenGender, StringComparison.InvariantCultureIgnoreCase) || stringGender.Equals(WomenGender, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("There is no such gender", $"{nameof(parameters.Gender)}");
            }
        }
    }
}
