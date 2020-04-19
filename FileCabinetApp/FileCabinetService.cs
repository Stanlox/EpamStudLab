using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            GuardClauses.IsNullOrEmpty(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckLength(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckDateRange(dateOfBirth, nameof(dateOfBirth));
            GuardClauses.CheckGender(gender, nameof(gender));
            GuardClauses.CheckSalarySign(salary, nameof(salary));
            GuardClauses.CheckAge(age, nameof(age));

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

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] array = new FileCabinetRecord[this.list.Count];
            array = this.list.ToArray();
            return array;
        }

        public int GetStat()
        {
           return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            this.firstNameDictionary.Remove(firstName);
            this.lastNameDictionary.Remove(lastName);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = id;
                record.FirstName = firstName;
                record.LastName = lastName;
                record.DateOfBirth = dateOfBirth;
                record.Gender = gender;
                record.Age = age;
                record.Salary = salary;
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord> { record });
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord> { record });
            }
        }

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

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime birthDate = DateTime.Parse(dateOfBirth, CultureInfo.CurrentCulture);
            if (this.dateofbirthDictionary.ContainsKey(birthDate))
            {
                this.list = this.dateofbirthDictionary[birthDate].ToList();
                FileCabinetRecord[] array = this.GetRecords();
                return array;
            }
            else
            {
                FileCabinetRecord[] array = Array.Empty<FileCabinetRecord>();
                return array;
            }
        }

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
