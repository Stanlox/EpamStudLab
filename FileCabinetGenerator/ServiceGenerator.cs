using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class ServiceGenerator
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly char[] arrayGender = { 'M', 'W' };
        private int randomYear;
        private int randomMonth;
        private int randomDay;
        private int index;

        public void GeneratorGenderAndDateOfBirth()
        {
            Random rnd = new Random();
            this.index = rnd.Next(this.arrayGender.Length - 1);
            this.randomYear = rnd.Next(MinDate.Year, DateTime.Now.Year);
            this.randomMonth = rnd.Next(1, 12);
            this.randomDay = rnd.Next(1, DateTime.DaysInMonth(this.randomYear, this.randomMonth));
        }

        public void CreateRecordRandomValues(int valueToStart, int amountOfGeneratedRecords)
        {
            while (valueToStart <= amountOfGeneratedRecords)
            {
                this.GeneratorGenderAndDateOfBirth();
                var record = new FileCabinetRecord
                {
                    Id = valueToStart,
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    DateOfBirth = new DateTime(this.randomYear, this.randomMonth, this.randomDay),
                    Salary = new Random().Next(0, int.MaxValue),
                    Age = (short)Faker.RandomNumber.Next(0, 130),
                    Gender = this.arrayGender[this.index],
                };
                valueToStart++;
                this.list.Add(record);
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
        /// makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of <see cref="FileCabinetServiceSnapshot"/> as an array.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.Select(x => this.DeepCopy(x)).ToArray());
        }
    }
}
