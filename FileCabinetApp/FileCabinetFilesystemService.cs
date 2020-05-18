using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int StringLengthSize = sizeof(long);
        private const int MaxStringLength = 30;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const int RecordSize = (IntSize * 4) + (StringLengthSize * 2) + DecimalSize + ShortSize + CharSize + (MaxStringLength * 2);
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private int recordId = 0;

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
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

        public void CheckUsersDataEntry(FileCabinetServiceContext objectParameter)
        {
            throw new NotImplementedException();
        }

        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (file.Length <= 0)
                {
                    this.recordId++;
                }
                else
                {
                    byte[] recordBuffer = new byte[file.Length];
                    file.Read(recordBuffer, 0, recordBuffer.Length);
                    this.list = this.BytesToFileCabinetRecord(recordBuffer);
                    this.recordId = this.list.Max(maxRecordID => maxRecordID.Id) + 1;
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

        public void FileCabinetRecordToBytes(FileCabinetRecord record)
        {
            var bytes = new byte[RecordSize];
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
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
                    binarywriter.Seek(0, SeekOrigin.End);
                    binarywriter.Write(this.recordId);
                    binarywriter.Write(record.Age);
                    binarywriter.Write(record.Salary);
                    binarywriter.Write(record.Gender);
                    binarywriter.Write(firstNameLength);
                    binarywriter.Write(firstNameBuffer);
                    binarywriter.Write(lastNameLength);
                    binarywriter.Write(lastNameBuffer);
                    binarywriter.Write(record.DateOfBirth.Year);
                    binarywriter.Write(record.DateOfBirth.Month);
                    binarywriter.Write(record.DateOfBirth.Day);
                }
            }
        }

        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {

            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);

                this.list = this.BytesToFileCabinetRecord(recordBuffer);
                var updateRecord = this.list.Find(record => record.Id == id);
                FileCabinetRecord oldrecord = updateRecord;
                this.RemoveRecordInFirstNameDictionary(oldrecord);
                this.RemoveRecordInLastNameDictionary(oldrecord);
                //this.RemoveRecordInDateOfBirthDictionary(oldrecord);
                updateRecord.Id = id;
                updateRecord.Age = objectParameter.Age;
                updateRecord.Salary = objectParameter.Salary;
                updateRecord.Gender = objectParameter.Gender;
                updateRecord.FirstName = objectParameter.FirstName;
                updateRecord.LastName = objectParameter.LastName;
                updateRecord.DateOfBirth = objectParameter.DateOfBirth;
                int offset = (RecordSize * id) - RecordSize;
                var bytes = new byte[RecordSize];

                using (var memoryStream = new MemoryStream(bytes))
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    var firstNameBytes = Encoding.ASCII.GetBytes(updateRecord.FirstName);
                    var lastNameBytes = Encoding.ASCII.GetBytes(updateRecord.LastName);
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
                        binarywriter.Seek(offset, SeekOrigin.Begin);
                        binarywriter.Write(id);
                        binarywriter.Write(updateRecord.Age);
                        binarywriter.Write(updateRecord.Salary);
                        binarywriter.Write(updateRecord.Gender);
                        binarywriter.Write(firstNameLength);
                        binarywriter.Write(firstNameBuffer);
                        binarywriter.Write(lastNameLength);
                        binarywriter.Write(lastNameBuffer);
                        binarywriter.Write(updateRecord.DateOfBirth.Year);
                        binarywriter.Write(updateRecord.DateOfBirth.Month);
                        binarywriter.Write(updateRecord.DateOfBirth.Day);
                    }
                }
            }
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.firstNameDictionary.Clear();
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = this.BytesToFileCabinetRecord(recordBuffer);
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

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.lastNameDictionary.Clear();
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = this.BytesToFileCabinetRecord(recordBuffer);
                foreach (var record in this.list)
                {
                    this.AddInDictionaryLastName(record.FirstName, record);
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

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using (var file2 = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file2.Length];
                file2.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = this.BytesToFileCabinetRecord(recordBuffer);
            }

            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return records;
        }

        public List<FileCabinetRecord> BytesToFileCabinetRecord(byte[] bytes)
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
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        FileCabinetRecord record = new FileCabinetRecord();
                        record.Id = binaryReader.ReadInt32();
                        record.Age = binaryReader.ReadInt16();
                        record.Salary = binaryReader.ReadDecimal();
                        record.Gender = binaryReader.ReadChar();
                        var firstNameLength = binaryReader.ReadInt32();
                        var firstNameBuffer = binaryReader.ReadBytes(MaxStringLength);
                        var lastNameLength = binaryReader.ReadInt32();
                        var lastNameBuffer = binaryReader.ReadBytes(MaxStringLength);

                        record.FirstName = Encoding.ASCII.GetString(firstNameBuffer, 0, firstNameLength);
                        record.LastName = Encoding.ASCII.GetString(lastNameBuffer, 0, lastNameLength);

                        var yearOfBirth = binaryReader.ReadInt32();
                        var monthOfBirth = binaryReader.ReadInt32();
                        var dayOfBirth = binaryReader.ReadInt32();
                        record.DateOfBirth = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);
                        recordList.Add(record);
                    }
                }
            }

            return recordList;
        }

        public int GetStat()
        {
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] recordBuffer = new byte[file.Length];
                file.Read(recordBuffer, 0, recordBuffer.Length);
                this.list = this.BytesToFileCabinetRecord(recordBuffer);
            }

            return this.list.Count;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        public void RemoveRecordInDateOfBirthDictionary(FileCabinetRecord oldRecord)
        {
            throw new NotImplementedException();
        }

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

        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
