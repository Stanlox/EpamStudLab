﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// contains services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetService : IRecordValidator
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Gets or sets properties ContextStrategy.
        /// </summary>
        /// <value>The ContextStrategy property gets/sets the value type of IRecordValidator.</value>
        public IRecordValidator ContextStrategy { get; set; }

        /// <summary>
        /// Implementation of the strategy pattern. Installation strategy.
        /// </summary>
        /// <param name="strategy"> Installation strategy.</param>
        public void CreateValidator(IRecordValidator strategy)
        {
            this.ContextStrategy = strategy;
        }

        /// <summary>
        /// creates a new records.
        /// </summary>
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            this.ContextStrategy.CheckUsersDataEntry(objectParameter);
            this.CheckUsersDataEntry(objectParameter);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = objectParameter.FirstName,
                LastName = objectParameter.LastName,
                DateOfBirth = objectParameter.DateOfBirth,
                Gender = objectParameter.Gender,
                Age = objectParameter.Age,
                Salary = objectParameter.Salary,
            };
            this.AddInDictionaryFirstName(objectParameter.FirstName, record);
            this.AddInDictionaryLastName(objectParameter.LastName, record);
            this.AddInDictionaryDateOfBirth(objectParameter.DateOfBirth, record);
            this.list.Add(record);
            return record.Id;
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] array = new FileCabinetRecord[this.list.Count];
            array = this.list.ToArray();
            return array;
        }

        /// <summary>
        /// gets statistics by records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public int GetStat()
        {
           return this.list.Count;
        }

        /// <summary>
        /// changes data an existing record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            this.CheckUsersDataEntry(objectParameter);

            FileCabinetRecord oldrecord = this.list[id - 1];
            this.RemoveRecordInFirstNameDictionary(oldrecord);
            this.RemoveRecordInLastNameDictionary(oldrecord);
            this.RemoveRecordInDateOfBirthDictionary(oldrecord);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = objectParameter.Id;
                record.FirstName = objectParameter.FirstName;
                record.LastName = objectParameter.LastName;
                record.DateOfBirth = objectParameter.DateOfBirth;
                record.Gender = objectParameter.Gender;
                record.Age = objectParameter.Age;
                record.Salary = objectParameter.Salary;
                this.AddInDictionaryFirstName(objectParameter.FirstName, record);
                this.AddInDictionaryLastName(objectParameter.LastName, record);
                this.AddInDictionaryDateOfBirth(objectParameter.DateOfBirth, record);
            }
        }

        /// <summary>
        /// remove record in <see cref="firstNameDictionary"/>.
        /// </summary>
        /// <param name="oldRecord">the record that is being modified.</param>
        /// <exception cref="ArgumentNullException">thrown when record is null.</exception>
        public void RemoveRecordInFirstNameDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.firstNameDictionary.ContainsKey(oldRecord.FirstName))
            {
                this.firstNameDictionary[oldRecord.FirstName].Remove(oldRecord);
            }
        }

        /// <summary>
        /// remove record in <see cref="lastNameDictionary"/>.
        /// </summary>
        /// <param name="oldRecord">the record that is being modified.</param>
        /// <exception cref="ArgumentNullException">thrown when record is null.</exception>
        public void RemoveRecordInLastNameDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.lastNameDictionary.ContainsKey(oldRecord.LastName))
            {
                this.lastNameDictionary[oldRecord.LastName].Remove(oldRecord);
            }
        }

        /// <summary>
        /// remove record in <see cref="dateofbirthDictionary"/>.
        /// </summary>
        /// <param name="oldRecord">the record that is being modified.</param>
        /// <exception cref="ArgumentNullException">thrown when record is null.</exception>
        public void RemoveRecordInDateOfBirthDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.dateofbirthDictionary.ContainsKey(oldRecord.DateOfBirth))
            {
                this.dateofbirthDictionary[oldRecord.DateOfBirth].Remove(oldRecord);
            }
        }

        /// <summary>
        /// find record in dictionary by FirstName.
        /// </summary>
        /// <param name="firstName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.list = this.firstNameDictionary[firstName].ToList();
                FileCabinetRecord[] array = this.GetRecords();
                return array;
            }
            else
            {
                FileCabinetRecord[] array = Array.Empty<FileCabinetRecord>();
                return array;
            }
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.list = this.lastNameDictionary[lastName].ToList();
                FileCabinetRecord[] array = this.GetRecords();
                return array;
            }
            else
            {
                FileCabinetRecord[] array = Array.Empty<FileCabinetRecord>();
                return array;
            }
        }

        /// <summary>
        /// find record in dictionary by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">the key for search.</param>
        /// <returns>found a list of records.</returns>
        /// <exception cref="ArgumentException">throw when date is not correct.</exception>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime dateValue;
            bool birthDate = DateTime.TryParse(dateOfBirth, out dateValue);
            if (!birthDate)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(dateOfBirth)}");
            }

            if (this.dateofbirthDictionary.ContainsKey(dateValue))
            {
                this.list = this.dateofbirthDictionary[dateValue].ToList();
                FileCabinetRecord[] array = this.GetRecords();
                return array;
            }
            else
            {
                FileCabinetRecord[] array = Array.Empty<FileCabinetRecord>();
                return array;
            }
        }

        /// <summary>
        /// adds a dictionary record by key <paramref name="firstName"/>.
        /// </summary>
        /// <param name="firstName">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryFirstName(string firstName, FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// adds a dictionary record by key <paramref name="lastName"/>.
        /// </summary>
        /// <param name="lastName">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryLastName(string lastName, FileCabinetRecord record)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// adds a dictionary record by key <paramref name="dateofbirth"/>.
        /// </summary>
        /// <param name="dateofbirth">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
            if (this.dateofbirthDictionary.ContainsKey(dateofbirth))
            {
                this.dateofbirthDictionary[dateofbirth].Add(record);
            }
            else
            {
                this.dateofbirthDictionary.Add(dateofbirth, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// virtual method for checking the correctness of user input.
        /// </summary>
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void CheckUsersDataEntry(FileCabinetServiceContext objectParameter)
        {
        }
    }
}
