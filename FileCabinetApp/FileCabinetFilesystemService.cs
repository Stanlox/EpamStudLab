using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private FileStream fileStream;

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}
