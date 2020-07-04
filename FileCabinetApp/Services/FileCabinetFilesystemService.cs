using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Services.Comparer;
using FileCabinetApp.Services.Extensions;
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
        private readonly SortedDictionary<int, int> idrecordDictionary = new SortedDictionary<int, int>();
        private readonly SortedDictionary<string, List<int>> firstNameDictionary = new SortedDictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<string, List<int>> lastNameDictionary = new SortedDictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<DateTime, List<int>> dateofbirthDictionary = new SortedDictionary<DateTime, List<int>>();
        private readonly SortedDictionary<decimal, List<int>> salaryDictionary = new SortedDictionary<decimal, List<int>>();
        private readonly SortedDictionary<short, List<int>> ageDictionary = new SortedDictionary<short, List<int>>();
        private readonly SortedDictionary<char, List<int>> genderDictionary = new SortedDictionary<char, List<int>>(new CharComparer());
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
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            BytesToModel.SetFields(ref status, MaxStringLength, ref isDeletedlist);

            if (this.fileStream.Length != 0)
            {
                byte[] recordBuffer = new byte[this.fileStream.Length];
                this.fileStream.Read(recordBuffer, 0, recordBuffer.Length);
                var listRecord = recordBuffer.ToListFileCabinetRecord();
                var index = 1;
                for (int i = 0; i < listRecord.Count; i++)
                {
                    this.AddRecordInAllDictionary(listRecord[i], index);
                    index++;
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
        /// Adds a dictionary record by key <paramref name="salary"/>.
        /// </summary>
        /// <param name="salary">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionarySalary(decimal salary, int offsetFromBeginFile)
        {
            if (this.salaryDictionary.ContainsKey(salary))
            {
                this.salaryDictionary[salary].Add(offsetFromBeginFile);
            }
            else
            {
                this.salaryDictionary.Add(salary, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="age"/>.
        /// </summary>
        /// <param name="age">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryAge(short age, int offsetFromBeginFile)
        {
            if (this.ageDictionary.ContainsKey(age))
            {
                this.ageDictionary[age].Add(offsetFromBeginFile);
            }
            else
            {
                this.ageDictionary.Add(age, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="gender"/>.
        /// </summary>
        /// <param name="gender">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryGender(char gender, int offsetFromBeginFile)
        {
            if (this.genderDictionary.ContainsKey(gender))
            {
                this.genderDictionary[gender].Add(offsetFromBeginFile);
            }
            else
            {
                this.genderDictionary.Add(gender, new List<int> { offsetFromBeginFile });
            }
        }

        /// <summary>
        /// Adds a dictionary record by key <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Input key.</param>
        /// <param name="offsetFromBeginFile">Position in file.</param>
        public void AddInDictionaryIdRecord(int id, int offsetFromBeginFile)
        {
            this.idrecordDictionary[id] = offsetFromBeginFile;
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
        /// Creates a new records.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>Id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

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
                this.AddRecordInAllDictionary(record, ((int)this.fileStream.Length / RecordSize) + 1);
                this.FileCabinetRecordToBytes(record);
                return this.recordId;
            }
        }

        /// <summary>
        /// Converts an input record to bytes.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
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
        /// Changing data in an existing record.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
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

                this.list = recordBuffer.ToListFileCabinetRecord();
                var updateRecord = this.list.Find(record => record.Id == id);
                FileCabinetRecord oldrecord = updateRecord;
                var positionInFile = this.GetPositionInFileById(oldrecord.Id);
                this.RemoveRecordInAllDictionary(oldrecord, positionInFile);
                updateRecord.Id = id;
                updateRecord.Age = parameters.Age;
                updateRecord.Salary = parameters.Salary;
                updateRecord.Gender = parameters.Gender;
                updateRecord.FirstName = parameters.FirstName;
                updateRecord.LastName = parameters.LastName;
                updateRecord.DateOfBirth = parameters.DateOfBirth;
                this.AddRecordInAllDictionary(updateRecord, positionInFile);
                this.FileCabinetRecordToBytes(updateRecord);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            var isFinded = this.dateofbirthDictionary.TryGetValue(dateOfBirth, out List<int> positionList);

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

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(decimal salary)
        {
            var isFinded = this.salaryDictionary.TryGetValue(salary, out List<int> positionList);

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

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByAge(short age)
        {
            var isFinded = this.ageDictionary.TryGetValue(age, out List<int> positionList);

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
        /// Finds records by the specified id.
        /// </summary>
        /// <param name="id">Input id.</param>
        /// <returns>Found record.</returns>
        public FileCabinetRecord FindById(int id)
        {
            return this.idrecordDictionary.TryGetValue(id, out int recordPosition) ? this.ReadByPosition(recordPosition) : null;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            var isFinded = this.genderDictionary.TryGetValue(gender, out List<int> positionList);

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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = recordBuffer.ToListFileCabinetRecord();
                ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
                return records;
            }
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = recordBuffer.ToListFileCabinetRecord();
                return Tuple.Create(this.list.Count, isDeletedlist.Count);
            }
        }

        /// <inheritdoc/>
        public Tuple<int, int> PurgeRecord()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = recordBuffer.ToListFileCabinetRecord();
            }

            this.fileStream.SetLength(0);

            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    var position = i + 1;
                    this.AddRecordInAllDictionary(this.list[i], position);
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
        /// Сlearing all dictionaries.
        /// </summary>
        public void СlearingAllDictionaries()
        {
            this.idrecordDictionary.Clear();
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateofbirthDictionary.Clear();
            this.salaryDictionary.Clear();
            this.ageDictionary.Clear();
            this.genderDictionary.Clear();
        }

        /// <summary>
        /// Remove record in <see cref="dateofbirthDictionary"/>.
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
        /// Remove record in <see cref="firstNameDictionary"/>.
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
        /// Remove record in <see cref="salaryDictionary"/>.
        /// </summary>
        /// <param name="salary">Input key salary.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInSalaryDictionary(decimal salary, int positionInFile)
        {
            if (this.salaryDictionary.ContainsKey(salary))
            {
                this.salaryDictionary[salary].Remove(positionInFile);
            }
        }

        /// <summary>
        /// Remove record in <see cref="idrecordDictionary"/>.
        /// </summary>
        /// <param name="id">Input key salary.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInIdRecordDictionary(int id, int positionInFile)
        {
            if (this.idrecordDictionary.ContainsKey(id))
            {
                this.idrecordDictionary.Remove(positionInFile);
            }
        }

        /// <summary>
        /// Remove record in <see cref="ageDictionary"/>.
        /// </summary>
        /// <param name="age">Input key age.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInAgeDictionary(short age, int positionInFile)
        {
            if (this.ageDictionary.ContainsKey(age))
            {
                this.ageDictionary[age].Remove(positionInFile);
            }
        }

        /// <summary>
        /// Remove record in <see cref="genderDictionary"/>.
        /// </summary>
        /// <param name="gender">Input key gender.</param>
        /// <param name="positionInFile">Input position in file.</param>
        public void RemoveRecordInGenderDictionary(char gender, int positionInFile)
        {
            if (this.genderDictionary.ContainsKey(gender))
            {
                this.genderDictionary[gender].Remove(positionInFile);
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
        /// Remove record in all the dictionaries.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <param name="index">Position in file.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void RemoveRecordInAllDictionary(FileCabinetRecord record, int index)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.RemoveRecordInIdRecordDictionary(record.Id, index);
            this.RemoveRecordInFirstNameDictionary(record.FirstName, index);
            this.RemoveRecordInLastNameDictionary(record.LastName, index);
            this.RemoveRecordInDateOfBirthDictionary(record.DateOfBirth, index);
            this.RemoveRecordInSalaryDictionary(record.Salary, index);
            this.RemoveRecordInGenderDictionary(record.Gender, index);
            this.RemoveRecordInAgeDictionary(record.Age, index);
        }

        /// <summary>
        /// Add record in all the dictionaries.
        /// </summary>
        /// <param name="record">The record that is being modified.</param>
        /// <param name="index">Position in file.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void AddRecordInAllDictionary(FileCabinetRecord record, int index)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.AddInDictionaryIdRecord(record.Id, index);
            this.AddInDictionaryFirstName(record.FirstName, index);
            this.AddInDictionaryLastName(record.LastName, index);
            this.AddInDictionaryDateOfBirth(record.DateOfBirth, index);
            this.AddInDictionaryGender(record.Gender, index);
            this.AddInDictionarySalary(record.Salary, index);
            this.AddInDictionaryAge(record.Age, index);
        }

        /// <inheritdoc/>
        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

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
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = recordBuffer.ToListFileCabinetRecord();
                return new FileCabinetServiceSnapshot(this.list.Select(x => this.DeepCopy(x)).ToArray());
            }
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
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
                var record = this.ReadByPosition(findIdRecord);
                var positionInFile = this.GetPositionInFileById(record.Id);
                this.RemoveRecordInAllDictionary(record, positionInFile);
                status = 1;
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

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

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
                            this.EditRecord(recordFromFile[i].Id, fileCabinetServiceContext);
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
                        this.AddRecordInAllDictionary(fileCabinetRecord, ((int)this.fileStream.Length / RecordSize) + 1);
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
        /// Read record from file by position.
        /// </summary>
        /// <param name="position">Position in file.</param>
        /// <returns>Found record.</returns>
        public FileCabinetRecord ReadByPosition(int position)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Seek((RecordSize * position) - RecordSize, SeekOrigin.Begin);
                var bytes = new byte[RecordSize];
                file.Read(bytes, 0, RecordSize);
                var record = bytes.ToFileCabinetRecord();
                return record;
            }
        }
    }
}