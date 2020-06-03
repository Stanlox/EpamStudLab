using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomFirstNameValidator
    {
        /// <summary>
        /// checks the string for null and empty.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown when string is not corrct length.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.FirstName.Length < 2 | parameters.FirstName.Length > 15)
            {
                throw new ArgumentException("Invalid length.", $"{nameof(parameters.FirstName)}");
            }
        }
    }
}
