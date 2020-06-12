using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// Defines the execution time of the service methods.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private Stopwatch stopwatch = new Stopwatch();
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// initializes the service type.
        /// </summary>
        /// <param name="service">Input type of service.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Creates a new records.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext parameters)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.CreateRecord(parameters);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.CreateRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <returns>new new cloned object <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            FileCabinetRecord cabinetRecord = this.service.DeepCopy(record);
            return cabinetRecord;
        }

        /// <summary>
        /// Сhanging data in an existing record.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="parameters">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext parameters)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.EditRecord(id, parameters);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.EditRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// find record in dictionary by dateOfBirth.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="dateOfBirth">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IRecordIterator FindByDateOfBirth(string dateOfBirth)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByDateOfBirth)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// find record in dictionary by FirstName.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="firstName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByFirstName)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// find record in dictionary by LastName.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByLastName)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Gets records.
        /// Displays the run time of the method.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.GetRecords();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.GetRecords)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// gets statistics by records.
        /// Displays the run time of the method.
        /// </summary>
        /// <returns>Count of records.</returns>
        public Tuple<int, int> GetStat()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.GetStat();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.GetStat)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        /// <summary>
        /// clears records marked with the delete bits.
        /// Displays the run time of the method.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public Tuple<int, int> PurgeRecord()
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.PurgeRecord();
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.PurgeRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        /// <summary>
        /// Remove record by id.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="id">Input id record.</param>
        public void RemoveRecord(int id)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.RemoveRecord(id);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.RemoveRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Restore data.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="snapshot">Input object to retrieve a list of records.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.Restore)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
            return snapshot;
        }
    }
}
