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
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private int lastRecordId = 1;
        private int countRecordInFile = 0;

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
        }

        public void AddInDictionaryFirstName(string firstName, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        public void AddInDictionaryLastName(string lastName, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        public void CheckUsersDataEntry(FileCabinetServiceContext objectParameter)
        {
            throw new NotImplementedException();
        }

        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            var bytes = new byte[RecordSize];
            var recordBuffer = new byte[RecordSize];
            using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.countRecordInFile = (int)file.Length / RecordSize;
                var offset = (this.countRecordInFile - 1) * RecordSize;
                if (offset < 0)
                {
                }
                else
                {
                    file.Seek(offset, SeekOrigin.Begin);
                    file.Read(recordBuffer, 0, RecordSize);
                    this.lastRecordId = this.GetFileCabinetRecordIdToBytes(recordBuffer);
                }
            }

            using (var memoryStream = new MemoryStream(bytes))

            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                var firstNameBytes = Encoding.ASCII.GetBytes(objectParameter.FirstName);
                var lastNameBytes = Encoding.ASCII.GetBytes(objectParameter.LastName);
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

                using (BinaryWriter binaryWriter1 = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                {
                    binaryWriter1.Write(this.lastRecordId);
                    binaryWriter1.Write(objectParameter.Age);
                    binaryWriter1.Write(objectParameter.Salary);
                    binaryWriter1.Write(objectParameter.Gender);
                    binaryWriter1.Write(firstNameLength);
                    binaryWriter1.Write(firstNameBuffer);
                    binaryWriter1.Write(lastNameLength);
                    binaryWriter1.Write(lastNameBuffer);
                    binaryWriter1.Write(objectParameter.DateOfBirth.Year);
                    binaryWriter1.Write(objectParameter.DateOfBirth.Month);
                    binaryWriter1.Write(objectParameter.DateOfBirth.Day);
                }

                this.countRecordInFile++;
            }

            return this.lastRecordId;
        }

        public int GetFileCabinetRecordIdToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            FileCabinetRecord record = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                record.Id = binaryReader.ReadInt32();
            }

            return record.Id;
        }

        public byte[] FileCabinetRecordToBytes(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var bytes = new byte[RecordSize];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(record.Id);
                binaryWriter.Write(record.Age);
                binaryWriter.Write(record.Salary);
                binaryWriter.Write(record.Gender);
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

                binaryWriter.Write(firstNameLength);
                binaryWriter.Write(firstNameBuffer);
                binaryWriter.Write(lastNameLength);
                binaryWriter.Write(lastNameBuffer);

                binaryWriter.Write(record.DateOfBirth.Year);
                binaryWriter.Write(record.DateOfBirth.Month);
                binaryWriter.Write(record.DateOfBirth.Day);
            }

            return bytes;
        }

        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
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
            //if (countRecordInFile == 1)
            //{
            //    return countRecordInFile;
            //}
            //else if (countRecordInFile == 0)
            //{
            //    return countRecordInFile;
            //}
            //using (var file = File.Open(this.fileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //{
            //    this.countRecordInFile = ((int)file.Length / RecordSize) + 1;
            //}

            //return this.countRecordInFile;
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void RemoveRecordInLastNameDictionary(FileCabinetRecord oldRecord)
        {
            throw new NotImplementedException();
        }

        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
