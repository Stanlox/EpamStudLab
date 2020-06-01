using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// interface, that contains a single method for checking user input.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// method for checking user input.
        /// </summary>
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        void CheckUsersDataEntry(FileCabinetServiceContext objectParameter);
    }
}