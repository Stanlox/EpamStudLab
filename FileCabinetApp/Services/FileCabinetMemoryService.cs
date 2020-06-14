using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// contains memory services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetMemoryService : IRecordValidator, IFileCabinetService
    {
        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private readonly IRecordValidator contextStrategy;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="strategy">specific interface representative.</param>
        public FileCabinetMemoryService(IRecordValidator strategy)
        {
            this.contextStrategy = strategy;
        }

        /// <summary>
        /// makes a deep copy of the object.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <returns>new new cloned object <see cref="FileCabinetRecord"/>.</returns>
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

        /// <summary>
        /// makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.Select(x => this.DeepCopy(x)).ToArray());
        }

        /// <summary>
        /// creates a new records.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
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
            this.AddInDictionaryFirstName(parameters.FirstName, record);
            this.AddInDictionaryLastName(parameters.LastName, record);
            this.AddInDictionaryDateOfBirth(parameters.DateOfBirth, record);
            this.list.Add(record);
            return record.Id;
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return records;
        }

        /// <summary>
        /// gets statistics by records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public Tuple<int, int> GetStat()
        {
            return Tuple.Create(this.list.Count, 0);
        }

        /// <summary>
        /// changing data in an existing record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="parameters">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext parameters)
        {
            this.contextStrategy.ValidateParameters(parameters);

            FileCabinetRecord oldrecord = this.list[id - 1];
            this.RemoveRecordInFirstNameDictionary(oldrecord);
            this.RemoveRecordInLastNameDictionary(oldrecord);
            this.RemoveRecordInDateOfBirthDictionary(oldrecord);
            var record = this.list.Find(x => x.Id == id);
            record.Id = record.Id;
            record.FirstName = parameters.FirstName;
            record.LastName = parameters.LastName;
            record.DateOfBirth = parameters.DateOfBirth;
            record.Gender = parameters.Gender;
            record.Age = parameters.Age;
            record.Salary = parameters.Salary;
            this.AddInDictionaryFirstName(parameters.FirstName, record);
            this.AddInDictionaryLastName(parameters.LastName, record);
            this.AddInDictionaryDateOfBirth(parameters.DateOfBirth, record);
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
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.lastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        /// <summary>
        /// find record in dictionary by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">the key for search.</param>
        /// <returns>found a list of records.</returns>
        /// <exception cref="ArgumentException">throw when date is not correct.</exception>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            DateTime dateValue;
            bool birthDate = DateTime.TryParse(dateOfBirth, out dateValue);
            if (!birthDate)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(dateOfBirth)}");
            }

            return this.dateofbirthDictionary.TryGetValue(dateValue, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
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
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Input id record.</param>
        public void RemoveRecord(int id)
        {
            var removeRecord = this.list.Find(record => record.Id == id);
            this.list.Remove(removeRecord);
            this.RemoveRecordInDateOfBirthDictionary(removeRecord);
            this.RemoveRecordInFirstNameDictionary(removeRecord);
            this.RemoveRecordInLastNameDictionary(removeRecord);
        }

        /// <summary>
        /// virtual method for checking the correctness of user input.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
        }

        /// <summary>
        /// Restore data.
        /// </summary>
        /// <param name="snapshot">Input object to retrieve a list of records.</param>
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
                            this.list[i] = recordFromFile[j];
                            isFind = true;
                            break;
                        }
                    }

                   if (!isFind)
                   {
                       this.list.Add(recordFromFile[i]);
                   }

                   isFind = false;
                }
                catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
                {
                     Console.WriteLine($"{recordFromFile[i].Id}: {ex.Message}");
                }
            }
        }

        /// <summary>S
        /// Unrealized method.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public Tuple<int, int> PurgeRecord() => throw new NotImplementedException();

        /// <summary>
        /// Find record by id.
        /// </summary>
        /// <param name="id">Input id.</param>
        /// <returns>Found record.</returns>
        public FileCabinetRecord ReadByPosition(int id)
        {
            var record = this.list.Find(x => x.Id == id);
            return record;
        }
    }
}
