using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileCabinetApp.Services.Comparer;

namespace FileCabinetApp
{
    /// <summary>
    /// contains memory services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetMemoryService : IRecordValidator, IFileCabinetService
    {
        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private readonly IRecordValidator contextStrategy;
        private readonly Dictionary<decimal, List<FileCabinetRecord>> salaryDictionary = new Dictionary<decimal, List<FileCabinetRecord>>();
        private readonly Dictionary<short, List<FileCabinetRecord>> ageDictionary = new Dictionary<short, List<FileCabinetRecord>>();
        private readonly Dictionary<char, List<FileCabinetRecord>> genderDictionary = new Dictionary<char, List<FileCabinetRecord>>(new CharEqualityComparer());
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly Dictionary<int, FileCabinetRecord> idrecordDictionary = new Dictionary<int, FileCabinetRecord>();
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="strategy">specific interface representative.</param>
        public FileCabinetMemoryService(IRecordValidator strategy)
        {
            this.contextStrategy = strategy;
        }

        /// <inheritdoc/>
        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            return new FileCabinetRecord()
            {
                Id = record.Id,
                FirstName = record.FirstName,
                LastName = record.LastName,
                DateOfBirth = record.DateOfBirth,
                Salary = record.Salary,
                Gender = record.Gender,
                Age = record.Age,
            };
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.Select(x => this.DeepCopy(x)).ToArray());
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetServiceContext parameters)
        {
            this.contextStrategy.ValidateParameters(parameters);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count == 0 ? 1 : this.list.Max(maxId => maxId.Id) + 1,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                Gender = parameters.Gender,
                Age = parameters.Age,
                Salary = parameters.Salary,
            };
            this.AddRecordInAllDictionary(record);
            this.list.Add(record);
            return record.Id;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            return Tuple.Create(this.list.Count, 0);
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetServiceContext parameters)
        {
            this.contextStrategy.ValidateParameters(parameters);

            FileCabinetRecord oldrecord = this.list[id - 1];
            this.RemoveRecordInAllDictionary(oldrecord);
            var record = this.list.Find(x => x.Id == id);
            record.Id = record.Id;
            record.FirstName = parameters.FirstName;
            record.LastName = parameters.LastName;
            record.DateOfBirth = parameters.DateOfBirth;
            record.Gender = parameters.Gender;
            record.Age = parameters.Age;
            record.Salary = parameters.Salary;
            this.AddRecordInAllDictionary(record);
        }

        /// <summary>
        /// Remove record in <see cref="firstNameDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInFirstNameDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in <see cref="lastNameDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInLastNameDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in <see cref="dateofbirthDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInDateOfBirthDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.dateofbirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateofbirthDictionary[record.DateOfBirth].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in <see cref="salaryDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInSalaryDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.salaryDictionary.ContainsKey(record.Salary))
            {
                this.salaryDictionary[record.Salary].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in <see cref="ageDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInAgeDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.ageDictionary.ContainsKey(record.Age))
            {
                this.ageDictionary[record.Age].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in <see cref="idrecordDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInIdRecordDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.idrecordDictionary.ContainsKey(record.Id))
            {
                this.idrecordDictionary.Remove(record.Id);
            }
        }

        /// <summary>
        /// Remove record in <see cref="genderDictionary"/>.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInGenderDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            else if (this.genderDictionary.ContainsKey(record.Gender))
            {
                this.genderDictionary[record.Gender].Remove(record);
            }
        }

        /// <summary>
        /// Remove record in all the dictionaries.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInAllDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.RemoveRecordInFirstNameDictionary(record);
            this.RemoveRecordInLastNameDictionary(record);
            this.RemoveRecordInDateOfBirthDictionary(record);
            this.RemoveRecordInSalaryDictionary(record);
            this.RemoveRecordInGenderDictionary(record);
            this.RemoveRecordInAgeDictionary(record);
        }

        /// <summary>
        /// Add record in all the dictionaries.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void AddRecordInAllDictionary(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.AddInDictionaryId(record.Id, record);
            this.AddInDictionaryFirstName(record.FirstName, record);
            this.AddInDictionaryLastName(record.LastName, record);
            this.AddInDictionaryDateOfBirth(record.DateOfBirth, record);
            this.AddInDictionaryGender(record.Gender, record);
            this.AddInDictionarySalary(record.Salary, record);
            this.AddInDictionaryAge(record.Age, record);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.lastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.dateofbirthDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            return this.genderDictionary.TryGetValue(gender, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public FileCabinetRecord FindById(int id)
        {
            return this.idrecordDictionary.TryGetValue(id, out FileCabinetRecord result) ? result : null;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(decimal salary)
        {
            return this.salaryDictionary.TryGetValue(salary, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByAge(short age)
        {
            return this.ageDictionary.TryGetValue(age, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Adds a record to the dictionary by key <paramref name="firstName"/>.
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
        /// Adds a record to the dictionary by key <paramref name="lastName"/>.
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
        /// Adds a record to the dictionary by key <paramref name="dateofbirth"/>.
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
        /// Adds a record to the dictionary by key <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryId(int id, FileCabinetRecord record)
        {
            if (!this.idrecordDictionary.ContainsKey(id))
            {
                this.idrecordDictionary[id] = record;
            }
        }

        /// <summary>
        /// Adds a record to the dictionary by key <paramref name="salary"/>.
        /// </summary>
        /// <param name="salary">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionarySalary(decimal salary, FileCabinetRecord record)
        {
            if (this.salaryDictionary.ContainsKey(salary))
            {
                this.salaryDictionary[salary].Add(record);
            }
            else
            {
                this.salaryDictionary.Add(salary, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// Adds a record to the dictionary by key <paramref name="age"/>.
        /// </summary>
        /// <param name="age">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryAge(short age, FileCabinetRecord record)
        {
            if (this.ageDictionary.ContainsKey(age))
            {
                this.ageDictionary[age].Add(record);
            }
            else
            {
                this.ageDictionary.Add(age, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// Adds a record to the dictionary by key <paramref name="gender"/>.
        /// </summary>
        /// <param name="gender">Input key.</param>
        /// <param name="record">Input record.</param>
        public void AddInDictionaryGender(char gender, FileCabinetRecord record)
        {
            if (this.genderDictionary.ContainsKey(gender))
            {
                this.genderDictionary[gender].Add(record);
            }
            else
            {
                this.genderDictionary.Add(gender, new List<FileCabinetRecord> { record });
            }
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            var removeRecord = this.list.Find(record => record.Id == id);
            this.list.Remove(removeRecord);
            this.RemoveRecordInAllDictionary(removeRecord);
        }

        /// <summary>
        /// virtual method for checking the correctness of user input.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            var record = snapshot.Records;
            var recordFromFile = snapshot.ListFromFile;
            bool isFind = false;
            for (int i = 0; i < recordFromFile.Count; i++)
            {
                try
                {
                   fileCabinetServiceContext.FirstName = recordFromFile[i].FirstName;
                   fileCabinetServiceContext.LastName = recordFromFile[i].LastName;
                   fileCabinetServiceContext.DateOfBirth = recordFromFile[i].DateOfBirth;
                   fileCabinetServiceContext.Age = recordFromFile[i].Age;
                   fileCabinetServiceContext.Gender = recordFromFile[i].Gender;
                   fileCabinetServiceContext.Salary = recordFromFile[i].Salary;
                   this.contextStrategy.ValidateParameters(fileCabinetServiceContext);

                   for (int j = 0; j < record.Count; j++)
                   {
                        if (record[j].Id == recordFromFile[i].Id)
                        {
                            this.RemoveRecordInAllDictionary(this.list[i]);
                            this.list[i] = recordFromFile[j];
                            this.AddRecordInAllDictionary(this.list[i]);
                            isFind = true;
                            break;
                        }
                    }

                   if (!isFind)
                   {
                       this.list.Add(recordFromFile[i]);
                       this.AddRecordInAllDictionary(recordFromFile[i]);
                   }

                   isFind = false;
                }
                catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
                {
                     Console.WriteLine($"{recordFromFile[i].Id}: {ex.Message}");
                }
            }
        }

        /// <inheritdoc/>
        public Tuple<int, int> PurgeRecord() => throw new NotImplementedException();
    }
}
