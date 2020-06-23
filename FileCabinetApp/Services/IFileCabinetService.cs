using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Extract interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create record.
        /// </summary>
        /// <param name="objectParameter">Input objectParameter.</param>
        /// <returns>New record.</returns>
        int CreateRecord(FileCabinetServiceContext objectParameter);

        /// <summary>
        /// Edit record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input objectParameter.</param>
        void EditRecord(int id, FileCabinetServiceContext objectParameter);

        /// <summary>
        /// Finds records by the specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Finds records by the specified first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds records by the specified last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>list of record.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Get count of existing and deleted records.
        /// </summary>
        /// <returns>Count of record.</returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <returns>New cloned object <see cref="FileCabinetRecord"/>.</returns>
        FileCabinetRecord DeepCopy(FileCabinetRecord record);

        /// <summary>
        /// Makes a snapshot of an list.
        /// </summary>
        /// <returns>New cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restore data.
        /// </summary>
        /// <param name="snapshot">Input object to retrieve a list.</param>
        void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Input id record.</param>
        void RemoveRecord(int id);

        /// <summary>
        /// Deletes all records marked with the delete bits.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        Tuple<int, int> PurgeRecord();

        /// <summary>
        /// Finds records by the specified Id.
        /// </summary>
        /// <param name="position">Input position.</param>
        /// <returns>Found record.</returns>
        FileCabinetRecord FindById(int position);

        /// <summary>
        /// Finds records by the specified gender.
        /// </summary>
        /// <param name="gender">Gender to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindByGender(char gender);

        /// <summary>
        /// Finds records by the specified salary.
        /// </summary>
        /// <param name="salary">Salary to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindBySalary(decimal salary);

        /// <summary>
        /// Finds records by the specified age.
        /// </summary>
        /// <param name="age">Age to search.</param>
        /// <returns>Found records.</returns>
        IEnumerable<FileCabinetRecord> FindByAge(short age);
    }
}