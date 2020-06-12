using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// Extract interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="objectParameter">Input objectParameter.</param>
        /// <returns>New record.</returns>
        int CreateRecord(FileCabinetServiceContext objectParameter);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input objectParameter.</param>
        void EditRecord(int id, FileCabinetServiceContext objectParameter);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <returns>found a list of records.</returns>
        IRecordIterator FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="firstName">Input firstName.</param>
        /// <returns>found a list of records.</returns>
        IRecordIterator FindByFirstName(string firstName);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="lastName">Input lastName.</param>
        /// <returns>found a list of records.</returns>
        IRecordIterator FindByLastName(string lastName);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <returns>list of record.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <returns>Count of record.</returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// makes a deep copy of the object.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <returns>new new cloned object <see cref="FileCabinetRecord"/>.</returns>
        FileCabinetRecord DeepCopy(FileCabinetRecord record);

        /// <summary>
        /// makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
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
    }
}