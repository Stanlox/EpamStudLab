using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

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
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            this.contextStrategy.CheckUsersDataEntry(objectParameter);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count != 0 ? this.list.Count + 1 : 1,
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
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// changing data in an existing record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            this.contextStrategy.CheckUsersDataEntry(objectParameter);

            FileCabinetRecord oldrecord = this.list[id - 1];
            this.RemoveRecordInFirstNameDictionary(oldrecord);
            this.RemoveRecordInLastNameDictionary(oldrecord);
            this.RemoveRecordInDateOfBirthDictionary(oldrecord);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = record.Id;
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                List<FileCabinetRecord> listOfRecords = this.firstNameDictionary[firstName].ToList();
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(listOfRecords);
                return readOnlyCollection;
            }
            else
            {
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
                return readOnlyCollection;
            }
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                List<FileCabinetRecord> listOfRecords = this.lastNameDictionary[lastName].ToList();
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(listOfRecords);
                return readOnlyCollection;
            }
            else
            {
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
                return readOnlyCollection;
            }
        }

        /// <summary>
        /// find record in dictionary by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">the key for search.</param>
        /// <returns>found a list of records.</returns>
        /// <exception cref="ArgumentException">throw when date is not correct.</exception>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            DateTime dateValue;
            bool birthDate = DateTime.TryParse(dateOfBirth, out dateValue);
            if (!birthDate)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(dateOfBirth)}");
            }

            if (this.dateofbirthDictionary.ContainsKey(dateValue))
            {
                List<FileCabinetRecord> listOfRecords = this.dateofbirthDictionary[dateValue].ToList();
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(listOfRecords);
                return readOnlyCollection;
            }
            else
            {
                ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
                return readOnlyCollection;
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
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void CheckUsersDataEntry(FileCabinetServiceContext objectParameter)
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
            if (this.contextStrategy is CustomValidator)
            {
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
                        this.contextStrategy.CheckUsersDataEntry(fileCabinetServiceContext);
                        for (int j = 0; j < record.Count; j++)
                        {
                            if (record[j].Id == recordFromFile[i].Id)
                            {
                                this.list[j] = recordFromFile[i];
                                isFind = true;
                            }
                            else if (!isFind)
                            {
                                recordFromFile[i].Id = this.list.Count + 1;
                                this.list.Add(recordFromFile[i]);
                            }
                        }

                        isFind = false;
                    }
                    catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
                    {
                        Console.WriteLine($"{recordFromFile[i].Id} : {ex.Message}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < recordFromFile.Count; i++)
                {
                    for (int j = 0; j < record.Count; j++)
                    {
                        if (record[j].Id == recordFromFile[i].Id)
                        {
                            this.list[j] = recordFromFile[i];
                            isFind = true;
                        }
                        else if (!isFind)
                        {
                            recordFromFile[i].Id = this.list.Count + 1;
                            this.list.Add(recordFromFile[i]);
                        }
                    }

                    isFind = false;
                }
            }
        }

        /// <summary>
        /// Unrealized method.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public (int, int) PurgeRecord() => throw new NotImplementedException();
    }
}
