﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        /// <returns>Id of the new record.</returns>
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
        /// Creates a new records.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <param name="id">Input id record.</param>
        /// <returns>Id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext parameters, int id)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var rezult = this.service.CreateRecord(parameters, id);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.CreateRecord)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return rezult;
        }

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        /// <param name="record">Input record.</param>
        /// <returns>New cloned object <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord DeepCopy(FileCabinetRecord record)
        {
            FileCabinetRecord cabinetRecord = this.service.DeepCopy(record);
            return cabinetRecord;
        }

        /// <summary>
        /// Сhanging data in an existing record.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
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
        /// Find record in dictionary by dateOfBirth.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="dateOfBirth">The key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByDateOfBirth)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by id.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="id">t\The key for search.</param>
        /// <returns>Found record.</returns>
        public FileCabinetRecord FindById(int id)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindById(id);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindById)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by FirstName.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="firstName">The key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByFirstName)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by Salary.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="salary">The key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindBySalary(decimal salary)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindBySalary(salary);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindBySalary)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by Age.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="age">The key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindByAge(short age)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByAge(age);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByAge)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by Gender.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="gender">The key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            this.stopwatch.Reset();
            this.stopwatch.Start();
            var records = this.service.FindByGender(gender);
            this.stopwatch.Stop();
            Console.WriteLine($"{nameof(this.service.FindByGender)} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Find record in dictionary by LastName.
        /// Displays the run time of the method.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>Found record.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
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
        /// Clears records marked with the delete bits.
        /// Displays the run time of the method.
        /// </summary>
        /// <returns>Tuple number deleted records from total number records.</returns>
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
        /// <returns>New cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
            return snapshot;
        }
    }
}
