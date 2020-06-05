using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface, that contains a single method for checking user input.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Method for checking user input.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        void ValidateParameters(FileCabinetServiceContext parameters);
    }
}