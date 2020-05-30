using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// contains file services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 120;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const int RecordSize = (IntSize * 4) + DecimalSize + (ShortSize * 2) + CharSize + (MaxStringLength * 2) - 1;
        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private static short status = 0;
        private static List<FileCabinetRecord> isDeletedlist = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator contextStrategy = new DefaultValidator();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private int recordId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Input FileStream.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <summary>
        /// converts an array of bytes to a record.
        /// </summary>
        /// <param name="bytes">Input array of bytes.</param>
        /// <returns>list of records.</returns>
        public static List<FileCabinetRecord> BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            List<FileCabinetRecord> recordList = new List<FileCabinetRecord>();

            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    binaryReader.BaseStream.Position = 0;
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        FileCabinetRecord record = new FileCabinetRecord();
                        status = binaryReader.ReadInt16();
                        record.Id = binaryReader.ReadInt32();
                        var firstNameBuffer = binaryReader.ReadBytes(MaxStringLength);
                        var lastNameBuffer = binaryReader.ReadBytes(MaxStringLength);
                        var firstNameLength = Encoding.ASCII.GetString(firstNameBuffer, 0, firstNameBuffer.Length);
                        var lastNameLength = Encoding.ASCII.GetString(lastNameBuffer, 0, lastNameBuffer.Length);
                        record.FirstName = firstNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                        record.LastName = lastNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                        var yearOfBirth = binaryReader.ReadInt32();
                        var monthOfBirth = binaryReader.ReadInt32();
                        var dayOfBirth = binaryReader.ReadInt32();
                        record.DateOfBirth = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);
                        record.Age = binaryReader.ReadInt16();
                        record.Salary = binaryReader.ReadDecimal();
                        record.Gender = binaryReader.ReadChar();
                        if (status == 1)
                        {
                            var findRecord = isDeletedlist.Find(item => item.Id == record.Id);
                            if (!isDeletedlist.Contains(findRecord))
                            {
                                isDeletedlist.Add(record);
                            }
                        }
                        else
                        {
                            recordList.Add(record);
                        }
                    }
                }
            }

            return recordList;
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
        /// creates a new records.
        /// </summary>
        /// <param name="objectParameter">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            this.contextStrategy.CheckUsersDataEntry(objectParameter);
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (file.Length <= 0)
                {
                    this.recordId++;
                }
                else
                {
                    byte[] recordBuffer = new byte[RecordSize];
                    int offset = (int)file.Length - RecordSize;
                    file.Seek(offset, SeekOrigin.Begin);
                    file.Read(recordBuffer, 0, recordBuffer.Length);
                    byte[] bufferStatus = new byte[sizeof(short)];
                    byte[] recordBufferId = new byte[sizeof(int)];
                    this.recordId = BitConverter.ToInt32(recordBuffer, bufferStatus.Length) + 1;
                }

                var record = new FileCabinetRecord
                {
                    Id = this.recordId,
                    Age = objectParameter.Age,
                    Salary = objectParameter.Salary,
                    Gender = objectParameter.Gender,
                    FirstName = objectParameter.FirstName,
                    LastName = objectParameter.LastName,
                    DateOfBirth = objectParameter.DateOfBirth,
                };

                this.FileCabinetRecordToBytes(record);
                return this.recordId;
            }
        }

        /// <summary>
        /// converts an input record to bytes.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <exception cref="ArgumentNullException">thrown when record is null.</exception>
        public void FileCabinetRecordToBytes(FileCabinetRecord record)
        {
            var bytes = new byte[RecordSize];
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            using (var memoryStream = new MemoryStream(bytes))
            using (var writer = new BinaryWriter(memoryStream))
            {
                var firstNameBytes = Encoding.ASCII.GetBytes(record.FirstName);
                var lastNameBytes = Encoding.ASCII.GetBytes(record.LastName);
                var firstNameBuffer = new byte[MaxStringLength];
                var lastNameBuffer = new byte[MaxStringLength];

                var lastNameLength = lastNameBytes.Length;
                var firstNameLength = firstNameBytes.Length;
                if (firstNameLength > MaxStringLength)
                {
                    firstNameLength = MaxStringLength;
                }

                if (lastNameLength > MaxStringLength)
                {
                    lastNameLength = MaxStringLength;
                }

                Array.Copy(firstNameBytes, 0, firstNameBuffer, 0, firstNameLength);
                Array.Copy(lastNameBytes, 0, lastNameBuffer, 0, lastNameLength);
                using (var binarywriter = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                {
                    if (this.list.Exists(x => x.Id == record.Id))
                    {
                        using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            int offset = (RecordSize * record.Id) - RecordSize;
                            file.Seek(offset, SeekOrigin.Begin);
                            byte[] bufferStatus = new byte[sizeof(short)];
                            byte[] recordBufferId = new byte[sizeof(int)];
                            file.Read(bufferStatus, 0, bufferStatus.Length);
                            status = BitConverter.ToInt16(bufferStatus, 0);
                            file.Read(recordBufferId, 0, recordBufferId.Length);
                            this.recordId = BitConverter.ToInt32(recordBufferId, 0);
                            binarywriter.Seek(offset, SeekOrigin.Begin);
                        }
                    }
                    else
                    {
                        status = 0;
                        binarywriter.Seek(0, SeekOrigin.End);
                    }

                    binarywriter.Write(status);
                    binarywriter.Write(this.recordId);
                    binarywriter.Write(firstNameBuffer);
                    binarywriter.Write(lastNameBuffer);
                    binarywriter.Write(record.DateOfBirth.Year);
                    binarywriter.Write(record.DateOfBirth.Month);
                    binarywriter.Write(record.DateOfBirth.Day);
                    binarywriter.Write(record.Age);
                    binarywriter.Write(record.Salary);
                    binarywriter.Write(record.Gender);
                    binarywriter.Flush();
                }
            }
        }

        /// <summary>
        /// changing data in an existing record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            this.contextStrategy.CheckUsersDataEntry(objectParameter);
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);

                this.list = BytesToFileCabinetRecord(recordBuffer);
                var updateRecord = this.list.Find(record => record.Id == id);
                FileCabinetRecord oldrecord = updateRecord;
                this.RemoveRecordInFirstNameDictionary(oldrecord);
                this.RemoveRecordInLastNameDictionary(oldrecord);
                this.RemoveRecordInDateOfBirthDictionary(oldrecord);
                updateRecord.Id = id;
                updateRecord.Age = objectParameter.Age;
                updateRecord.Salary = objectParameter.Salary;
                updateRecord.Gender = objectParameter.Gender;
                updateRecord.FirstName = objectParameter.FirstName;
                updateRecord.LastName = objectParameter.LastName;
                updateRecord.DateOfBirth = objectParameter.DateOfBirth;
                this.FileCabinetRecordToBytes(updateRecord);
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
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.dateofbirthDictionary.Clear();
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
                foreach (var record in this.list)
                {
                    this.AddInDictionaryDateOfBirth(record.DateOfBirth, record);
                }

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
        }

        /// <summary>
        /// find record in dictionary by FirstName.
        /// </summary>
        /// <param name="firstName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.firstNameDictionary.Clear();
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
                foreach (var record in this.list)
                {
                    this.AddInDictionaryFirstName(record.FirstName, record);
                }

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
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.lastNameDictionary.Clear();
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
                foreach (var record in this.list)
                {
                    this.AddInDictionaryLastName(record.LastName, record);
                }

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
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
            }

            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return records;
        }

        /// <summary>
        /// gets statistics by records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public Tuple<int, int> GetStat()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
            }

            return Tuple.Create(this.list.Count, isDeletedlist.Count);
        }

        /// <summary>
        /// clears records marked with the delete bits.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public Tuple<int, int> PurgeRecord()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
            }

            this.fileStream.SetLength(0);

            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    status = 0;
                    var firstNameBytes = Encoding.ASCII.GetBytes(this.list[i].FirstName);
                    var lastNameBytes = Encoding.ASCII.GetBytes(this.list[i].LastName);
                    var firstNameBuffer = new byte[MaxStringLength];
                    var lastNameBuffer = new byte[MaxStringLength];

                    var lastNameLength = lastNameBytes.Length;
                    var firstNameLength = firstNameBytes.Length;
                    Array.Copy(firstNameBytes, 0, firstNameBuffer, 0, firstNameLength);
                    Array.Copy(lastNameBytes, 0, lastNameBuffer, 0, lastNameLength);

                    writer.Write(status);
                    writer.Write(this.list[i].Id);
                    writer.Write(firstNameBuffer);
                    writer.Write(lastNameBuffer);
                    writer.Write(this.list[i].DateOfBirth.Year);
                    writer.Write(this.list[i].DateOfBirth.Month);
                    writer.Write(this.list[i].DateOfBirth.Day);
                    writer.Write(this.list[i].Age);
                    writer.Write(this.list[i].Salary);
                    writer.Write(this.list[i].Gender);
                    writer.Flush();
                }
            }

            var countRecordisDeletedlist = isDeletedlist.Count;
            isDeletedlist.Clear();

            return Tuple.Create(countRecordisDeletedlist, this.list.Count + countRecordisDeletedlist);
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
            else if (this.lastNameDictionary.ContainsKey(oldRecord.LastName))
            {
                this.lastNameDictionary[oldRecord.LastName].Remove(oldRecord);
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
            else if (this.firstNameDictionary.ContainsKey(oldRecord.FirstName))
            {
                this.firstNameDictionary[oldRecord.FirstName].Remove(oldRecord);
            }
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
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToFileCabinetRecord(recordBuffer);
                return new FileCabinetServiceSnapshot(this.list.Select(x => this.DeepCopy(x)).ToArray());
            }
        }

        /// <summary>
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Input id record.</param>
        public void RemoveRecord(int id)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                status = 1;
                int findIdRecord = 0;
                int offset = 0;
                byte[] recordBuffer = new byte[RecordSize];
                do
                {
                    file.Seek(offset, SeekOrigin.Begin);
                    recordBuffer = new byte[RecordSize];
                    file.Read(recordBuffer, 0, recordBuffer.Length);
                    offset = offset + RecordSize;
                    byte[] bufferStatus = new byte[sizeof(short)];
                    byte[] recordBufferId = new byte[sizeof(int)];
                    findIdRecord = BitConverter.ToInt32(recordBuffer, bufferStatus.Length);
                }
                while (findIdRecord != id);

                var bytes = BitConverter.GetBytes(status);
                Array.Copy(bytes, 0, recordBuffer, 0, 2);

                offset = offset - RecordSize;
                using (var binarywriter = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                {
                    binarywriter.Seek(offset, SeekOrigin.Begin);
                    binarywriter.Write(recordBuffer);
                }
            }
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
                    this.contextStrategy.CheckUsersDataEntry(fileCabinetServiceContext);
                    for (int j = 0; j < record.Count; j++)
                    {
                        if (record[j].Id == recordFromFile[i].Id)
                        {
                            this.EditRecord(this.list[i].Id, fileCabinetServiceContext);
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        this.recordId = recordFromFile[i].Id;
                        var fileCabinetRecord = new FileCabinetRecord
                        {
                            Id = recordFromFile[i].Id,
                            Age = fileCabinetServiceContext.Age,
                            Salary = fileCabinetServiceContext.Salary,
                            Gender = fileCabinetServiceContext.Gender,
                            FirstName = fileCabinetServiceContext.FirstName,
                            LastName = fileCabinetServiceContext.LastName,
                            DateOfBirth = fileCabinetServiceContext.DateOfBirth,
                        };

                        this.FileCabinetRecordToBytes(fileCabinetRecord);
                    }

                    isFind = false;
                }
                catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
                {
                     Console.WriteLine($"{recordFromFile[i].Id} : {ex.Message}");
                }
            }
        }
    }
}
