using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int StringLengthSize = sizeof(long);
        private const int MaxStringLength = 120;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const int RecordSize = (IntSize * 4) + (StringLengthSize * 2) + DecimalSize + ShortSize + CharSize;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;

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
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = objectParameter.FirstName,
                LastName = objectParameter.LastName,
                DateOfBirth = objectParameter.DateOfBirth,
                Gender = objectParameter.Gender,
                Age = objectParameter.Age,
                Salary = objectParameter.Salary,
            };
            var b1 = this.ObjectParameterToBytes(record);
            this.fileStream.Write(b1, 0, b1.Length);
            this.fileStream.Flush();
            this.list.Add(record);
            return record.Id;
        }

        public byte[] ObjectParameterToBytes(FileCabinetRecord record)
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
                binaryWriter.Write(BitConverter.GetBytes(record.Gender));
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

                binaryWriter.Write(firstNameBuffer);
                binaryWriter.Write(lastNameBuffer);

                binaryWriter.Write(record.DateOfBirth.Year);
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
            throw new NotImplementedException();
        }

        public int GetStat()
        {
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
