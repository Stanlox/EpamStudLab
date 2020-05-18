using System;
using System.Collections.ObjectModel;

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
        /// <param name="dateofbirth">Input dateofbirth.</param>
        /// <param name="record">Input record.</param>
        void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="firstName">Input firstName.</param>
        /// <param name="record">Input record.</param>
        void AddInDictionaryFirstName(string firstName, FileCabinetRecord record);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="lastName">Input lastName.</param>
        /// <param name="record">Input record.</param>
        void AddInDictionaryLastName(string lastName, FileCabinetRecord record);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="objectParameter">Input objectParameter.</param>
        void CheckUsersDataEntry(FileCabinetServiceContext objectParameter);

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
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="firstName">Input firstName.</param>
        /// <returns>found a list of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="lastName">Input lastName.</param>
        /// <returns>found a list of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <returns>list of record.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <returns>Count of record.</returns>
        int GetStat();

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="oldRecord">Input record for edit.</param>
        void RemoveRecordInDateOfBirthDictionary(FileCabinetRecord oldRecord);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="oldRecord">Input record for edit.</param>
        void RemoveRecordInFirstNameDictionary(FileCabinetRecord oldRecord);

        /// <summary>
        /// contains method signature.
        /// </summary>
        /// <param name="oldRecord">Input record for edit.</param>
        void RemoveRecordInLastNameDictionary(FileCabinetRecord oldRecord);

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
    }
}