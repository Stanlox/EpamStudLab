using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// contains file services for adding, editing, and modifying records using filestream.
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
        private readonly SortedDictionary<string, List<int>> firstNameDictionary = new SortedDictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<string, List<int>> lastNameDictionary = new SortedDictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<DateTime, List<int>> dateofbirthDictionary = new SortedDictionary<DateTime, List<int>>();
        private readonly IRecordValidator contextStrategy = new ValidatorBuilder().CreateDefault();
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

            if (this.fileStream.Length != 0)
            {
                byte[] recordBuffer = new byte[this.fileStream.Length];
                this.fileStream.Read(recordBuffer, 0, recordBuffer.Length);
                var listRecord = BytesToListFileCabinetRecord(recordBuffer);
                var index = 1;
                for (int i = 0; i < listRecord.Count; i++)
                {
                    this.AddInDictionaryFirstName(listRecord[i].FirstName, index);
                    this.AddInDictionaryLastName(listRecord[i].LastName, index);
                    this.AddInDictionaryDateOfBirth(listRecord[i].DateOfBirth, index);
                    index++;
                }
            }
        }

        /// <summary>
        /// converts an array of bytes to a record.
        /// </summary>
        /// <param name="bytes">Input array of bytes.</param>
        /// <returns>list of records.</returns>
        public static List<FileCabinetRecord> BytesToListFileCabinetRecord(byte[] bytes)
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
        /// Сonverts one byte to a single record.
        /// </summary>
        /// <param name="bytes">Input bytes.</param>
        /// <returns>Record.</returns>
        public static FileCabinetRecord BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    binaryReader.BaseStream.Position = 0;
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
                    return record;
                }
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="dateofbirth"/>.
        /// </summary>
        /// <param name="dateofbirth">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, int offsetFromBeginFile)
        {
            if (this.dateofbirthDictionary.ContainsKey(dateofbirth))
            {
                this.dateofbirthDictionary[dateofbirth].Add(offsetFromBeginFile);
            }
            else
            {
                this.dateofbirthDictionary.Add(dateofbirth, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="firstName"/>.
        /// </summary>
        /// <param name="firstName">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryFirstName(string firstName, int offsetFromBeginFile)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(offsetFromBeginFile);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="lastName"/>.
        /// </summary>
        /// <param name="lastName">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryLastName(string lastName, int offsetFromBeginFile)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(offsetFromBeginFile);
            }
            else
            {
                this.lastNameDictionary.Add(lastName, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// creates a new records.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext parameters)
        {
            this.contextStrategy.ValidateParameters(parameters);
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
                    Age = parameters.Age,
                    Salary = parameters.Salary,
                    Gender = parameters.Gender,
                    FirstName = parameters.FirstName,
                    LastName = parameters.LastName,
                    DateOfBirth = parameters.DateOfBirth,
                };

                this.AddInDictionaryFirstName(record.FirstName, ((int)this.fileStream.Length / RecordSize) + 1);
                this.AddInDictionaryLastName(record.LastName, ((int)this.fileStream.Length / RecordSize) + 1);
                this.AddInDictionaryDateOfBirth(record.DateOfBirth, ((int)this.fileStream.Length / RecordSize) + 1);
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
        /// <param name="parameters">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.contextStrategy.ValidateParameters(parameters);
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);

                this.list = BytesToListFileCabinetRecord(recordBuffer);
                var updateRecord = this.list.Find(record => record.Id == id);
                FileCabinetRecord oldrecord = updateRecord;
                var positionInFile = this.GetPositionInFileById(oldrecord.Id);
                this.RemoveRecordInFirstNameDictionary(oldrecord.FirstName, positionInFile);
                this.RemoveRecordInLastNameDictionary(oldrecord.LastName, positionInFile);
                this.RemoveRecordInDateOfBirthDictionary(oldrecord.DateOfBirth, positionInFile);
                this.AddInDictionaryFirstName(parameters.FirstName, positionInFile);
                this.AddInDictionaryLastName(parameters.LastName, positionInFile);
                this.AddInDictionaryDateOfBirth(parameters.DateOfBirth, positionInFile);
                updateRecord.Id = id;
                updateRecord.Age = parameters.Age;
                updateRecord.Salary = parameters.Salary;
                updateRecord.Gender = parameters.Gender;
                updateRecord.FirstName = parameters.FirstName;
                updateRecord.LastName = parameters.LastName;
                updateRecord.DateOfBirth = parameters.DateOfBirth;
                this.FileCabinetRecordToBytes(updateRecord);
            }
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

            var isFinded = this.dateofbirthDictionary.TryGetValue(dateValue, out List<int> positionList);

            if (!isFinded)
            {
                yield break;
            }

            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            foreach (var position in positionList)
            {
                var record = this.ReadByPosition(position);
                yield return record;
            }
        }

        /// <summary>
        /// find record in dictionary by FirstName.
        /// </summary>
        /// <param name="firstName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var isFinded = this.firstNameDictionary.TryGetValue(firstName, out List<int> positionList);

            if (!isFinded)
            {
                yield break;
            }

            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            foreach (var position in positionList)
            {
                var record = this.ReadByPosition(position);
                yield return record;
            }
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            var isFinded = this.lastNameDictionary.TryGetValue(lastName, out List<int> positionList);

            if (!isFinded)
            {
                yield break;
            }

            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            foreach (var position in positionList)
            {
                var record = this.ReadByPosition(position);
                yield return record;
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
                this.list = BytesToListFileCabinetRecord(recordBuffer);
                ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
                return records;
            }
        }

        /// <summary>
        /// Gets statistics by records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public Tuple<int, int> GetStat()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToListFileCabinetRecord(recordBuffer);
                return Tuple.Create(this.list.Count, isDeletedlist.Count);
            }
        }

        /// <summary>
        /// Clears records marked with the delete bits.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public Tuple<int, int> PurgeRecord()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToListFileCabinetRecord(recordBuffer);
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
        /// <param name="dateOfBirth">Input key dateOfBirth.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInDateOfBirthDictionary(DateTime dateOfBirth, int positionInFile)
        {
            if (this.dateofbirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateofbirthDictionary[dateOfBirth].Remove(positionInFile);
            }
        }

        /// <summary>
        /// remove record in <see cref="firstNameDictionary"/>.
        /// </summary>
        /// <param name="firstname">Input key firstname.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInFirstNameDictionary(string firstname, int positionInFile)
        {
            if (this.firstNameDictionary.ContainsKey(firstname))
            {
                this.firstNameDictionary[firstname].Remove(positionInFile);
            }
        }

        /// <summary>
        /// remove record in <see cref="lastNameDictionary"/>.
        /// </summary>
        /// <param name="lastname">Input key lastname.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInLastNameDictionary(string lastname, int positionInFile)
        {
            if (this.lastNameDictionary.ContainsKey(lastname))
            {
                this.lastNameDictionary[lastname].Remove(positionInFile);
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
        /// Makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = BytesToListFileCabinetRecord(recordBuffer);
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
                var record = this.list.Find(findRecord => findRecord.Id == findIdRecord);
                var positionInFile = this.GetPositionInFileById(record.Id);
                this.RemoveRecordInFirstNameDictionary(record.FirstName, positionInFile);
                this.RemoveRecordInLastNameDictionary(record.LastName, positionInFile);
                this.RemoveRecordInDateOfBirthDictionary(record.DateOfBirth, positionInFile);
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
                    this.contextStrategy.ValidateParameters(fileCabinetServiceContext);
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
                        this.AddInDictionaryFirstName(fileCabinetRecord.FirstName, ((int)this.fileStream.Length / RecordSize) + 1);
                        this.AddInDictionaryLastName(fileCabinetRecord.LastName, ((int)this.fileStream.Length / RecordSize) + 1);
                        this.AddInDictionaryDateOfBirth(fileCabinetRecord.DateOfBirth, ((int)this.fileStream.Length / RecordSize) + 1);
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

        /// <summary>
        /// Get position in file by id.
        /// </summary>
        /// <param name="id">Input id record.</param>
        /// <returns>position record in file.</returns>
        public int GetPositionInFileById(int id)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int position = 1;
                int offset = 0;
                byte[] recordBuffer = new byte[RecordSize];
                do
                {
                    file.Seek(offset, SeekOrigin.Begin);
                    recordBuffer = new byte[RecordSize];
                    file.Read(recordBuffer, 0, recordBuffer.Length);
                    offset += RecordSize;
                    byte[] bufferStatus = new byte[sizeof(short)];
                    byte[] recordBufferId = new byte[sizeof(int)];
                    var inFileId = BitConverter.ToInt32(recordBuffer, bufferStatus.Length);
                    if (inFileId == id)
                    {
                        return position;
                    }
                    else
                    {
                        position++;
                    }
                }
                while (file.CanRead);
                return -1;
            }
        }

        /// <summary>
        /// Read record by position in file.
        /// </summary>
        /// <param name="position">Input position record in file.</param>
        /// <returns>Record.</returns>
        public FileCabinetRecord ReadByPosition(int position)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Seek((RecordSize * position) - RecordSize, SeekOrigin.Begin);
                var bytes = new byte[RecordSize];
                file.Read(bytes, 0, RecordSize);
                var record = BytesToFileCabinetRecord(bytes);
                return record;
            }
        }
    }
}