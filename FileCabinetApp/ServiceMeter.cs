using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FileCabinetApp
{
    public class ServiceMeter : IFileCabinetService
    {
        private Stopwatch stopwatch = new Stopwatch();
        private IFileCabinetService service;

        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        public int CreateRecord(FileCabinetServiceContext objectParameter)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.CreateRecord(objectParameter);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.CreateRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            FileCabinetRecord cabinetRecord = this.service.DeepCopy(record);
            return cabinetRecord;
        }

        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.EditRecord(id, objectParameter);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.EditRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByDateOfBirth)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByFirstName)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByLastName)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.GetRecords();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.GetRecords)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        public Tuple<int, int> GetStat()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.GetStat();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.GetStat)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        public Tuple<int, int> PurgeRecord()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.PurgeRecord();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.PurgeRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        public void RemoveRecord(int id)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.RemoveRecord(id);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.RemoveRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.Restore)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
            return snapshot;
        }

        void IFileCabinetService.AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.AddInDictionaryFirstName(string firstName, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.AddInDictionaryLastName(string lastName, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.RemoveRecordInDateOfBirthDictionary(FileCabinetRecord oldRecord)
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.RemoveRecordInFirstNameDictionary(FileCabinetRecord oldRecord)
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.RemoveRecordInLastNameDictionary(FileCabinetRecord oldRecord)
        {
            throw new NotImplementedException();
        }
    }
}
