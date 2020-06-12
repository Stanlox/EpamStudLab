﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Writes information about method calls to a file.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private IFileCabinetService service;
        private string path = "LogService.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// initializes the service type.
        /// </summary>
        /// <param name="service">Input type of service.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = new ServiceMeter(service);
        }

        /// <summary>
        /// Creates a new records and writes the execution result to the log.
        /// </summary>
        /// <param name="parameters">Input FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext parameters)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.CreateRecord)}() with FirstName = '{parameters.FirstName}'," +
                       $" LastName = '{parameters.LastName}', DateOfBirth = '{parameters.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}' " +
                       $" Salary = '{parameters.Salary}', Gender = '{parameters.Gender}'");
                var rezult = this.service.CreateRecord(parameters);
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.CreateRecord)}() returned '{rezult}'");
                return rezult;
            }
        }

        /// <summary>
        /// Сhanging data in an existing record and writes the execution result to the log.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="parameters">Input new FirstName, LastName, DateOfBirth, Gender, Salary, Age.</param>
        public void EditRecord(int id, FileCabinetServiceContext parameters)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.EditRecord)}() with id = '{id}', FirstName = '{parameters.FirstName}'," +
                    $" LastName = '{parameters.LastName}', DateOfBirth = '{parameters.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                    $" Salary = '{parameters.Salary}', Gender = '{parameters.Gender}'");
                this.service.EditRecord(id, parameters);
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.EditRecord)}() changed a record by id = '{id}' ");
            }
        }

        /// <summary>
        /// find record in dictionary by dateOfBirth and writes the execution result to the log.
        /// </summary>
        /// <param name="dateOfBirth">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.FindByDateOfBirth)}() with DateOfBith = '{dateOfBirth}' ");
                var records = this.service.FindByDateOfBirth(dateOfBirth);
                if (records.Count != 0)
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByDateOfBirth)}() found record(s) by DateOfBith = '{dateOfBirth}' ");
                }
                else
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByDateOfBirth)}() did not find  record(s) by DateOfBith = '{dateOfBirth}' ");
                }

                return records;
            }
        }

        /// <summary>
        /// find record in dictionary by FirstName and writes the execution result to the log.
        /// </summary>
        /// <param name="firstName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.FindByFirstName)}() with FirstName = '{firstName}' ");
                var records = this.service.FindByFirstName(firstName);
                if (records.Count != 0)
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByFirstName)}() found record(s) by FirstName = '{firstName}' ");
                }
                else
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByFirstName)}() did not find  record(s) by FirstName = '{firstName}' ");
                }

                return records;
            }
        }

        /// <summary>
        /// find record in dictionary by LastName and writes the execution result to the log.
        /// </summary>
        /// <param name="lastName">the key for search.</param>
        /// <returns>found a list of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.FindByLastName)}() with LastName = '{lastName}' ");
                var records = this.service.FindByLastName(lastName);
                if (records.Count != 0)
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByLastName)}() found record(s) by LastName = '{lastName}' ");
                }
                else
                {
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.FindByLastName)}() did not find  record(s) by LastName = '{lastName}' ");
                }

                return records;
            }
        }

        /// <summary>
        /// Gets records and writes the execution result to the log.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.GetRecords)}() ");
                var records = this.service.GetRecords();
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.GetRecords)}() returned all existing records ");
                return records;
            }
        }

        /// <summary>
        /// Gets statistics by records and writes the execution result to the log.
        /// </summary>
        /// <returns>Count of records.</returns>
        public Tuple<int, int> GetStat()
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.GetStat)}() ");
                var records = this.service.GetStat();
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.GetStat)}()" +
                    $" returned count of deleted record(s) = '{records.Item2}' and total count of record(s) = '{records.Item1}' ");
                return records;
            }
        }

        /// <summary>
        /// Clears records marked with the delete bits and writes the execution result to the log.
        /// </summary>
        /// <returns>tuple number deleted records from total number records.</returns>
        public Tuple<int, int> PurgeRecord()
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.PurgeRecord)}() ");
                var rezult = this.service.PurgeRecord();
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.PurgeRecord)}()" +
                    $" '{rezult.Item1}' of '{rezult.Item2}' record(s) were purged ");
                return rezult;
            }
        }

        /// <summary>
        /// Remove record by id and writes the execution result to the log.
        /// </summary>
        /// <param name="id">Input id record.</param>
        public void RemoveRecord(int id)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.RemoveRecord)}() with id = {id} ");
                this.service.RemoveRecord(id);
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.RemoveRecord)}() record with id= {id} was successfully deleted");
            }
        }

        /// <summary>
        /// Restore data and writes the execution result to the log.
        /// </summary>
        /// <param name="snapshot">Input object to retrieve a list of records.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.Restore)}()");
                this.service.Restore(snapshot);
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.Restore)}() all records were added to existing records ");
            }
        }

        /// <summary>
        /// Makes a snapshot of an list and writes the execution result to the log.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            using (TextWriter writer = File.AppendText(this.path))
            {
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - Calling {nameof(this.service.MakeSnapshot)}()");
                FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
                writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)} - {nameof(this.service.MakeSnapshot)}() made snapshot list of the records ");
                return snapshot;
            }
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
    }
}