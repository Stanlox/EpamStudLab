using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// contains services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// checks the data entered by the user.
        /// </summary>
        /// <param name="firstName">Input firstName.</param>
        /// <param name="lastName">Input lastName.</param>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <param name="gender">Input gender.</param>
        /// <param name="age">Input age.</param>
        /// <param name="salary">Input salary.</param>
        public static void CheckUsersDataEntry(string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            GuardClauses.IsNullOrEmpty(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckLength(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckDateRange(dateOfBirth, nameof(dateOfBirth));
            GuardClauses.CheckGender(gender, nameof(gender));
            GuardClauses.CheckSalarySign(salary, nameof(salary));
            GuardClauses.CheckAge(age, nameof(age));
        }

        /// <summary>
        /// creates a new records.
        /// </summary>
        /// <param name="firstName">Input firstName.</param>
        /// <param name="lastName">Input lastName.</param>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <param name="gender">Input gender.</param>
        /// <param name="age">Input age.</param>
        /// <param name="salary">Input salary.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            CheckUsersDataEntry(firstName, lastName, dateOfBirth, gender, age,  salary);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Age = age,
                Salary = salary,
            };
            this.AddInDictionaryFirstName(firstName, record);
            this.AddInDictionaryLastName(lastName, record);
            this.AddInDictionaryDateOfBirth(dateOfBirth, record);
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
        /// <param name="firstName">Input new firstName.</param>
        /// <param name="lastName">Input new lastName.</param>
        /// <param name="dateOfBirth">Input new dateOfBirth.</param>
        /// <param name="gender">Input new gender.</param>
        /// <param name="age">Input new age.</param>
        /// <param name="salary">Input new salary.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            CheckUsersDataEntry(firstName, lastName, dateOfBirth, gender, age, salary);
            FileCabinetRecord oldrecord = this.list[id - 1];
            this.RemoveRecordInFirstNameDictionary(oldrecord);
            this.RemoveRecordInLastNameDictionary(oldrecord);
            this.RemoveRecordInDateOfBirthDictionary(oldrecord);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = id;
                record.FirstName = firstName;
                record.LastName = lastName;
                record.DateOfBirth = dateOfBirth;
                record.Gender = gender;
                record.Age = age;
                record.Salary = salary;
                this.AddInDictionaryFirstName(firstName, record);
                this.AddInDictionaryLastName(lastName, record);
                this.AddInDictionaryDateOfBirth(dateOfBirth, record);
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
    }
}
