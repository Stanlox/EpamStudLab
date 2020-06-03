using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultLastNameValidator
    {
        /// <summary>
        /// checks the string length.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">Thrown when string is not corrct length.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (parameters.LastName.Length < 2 | parameters.LastName.Length > 60)
            {
                throw new ArgumentException("Invalid length.", $"{nameof(parameters.LastName)}");
            }
        }
    }
}
