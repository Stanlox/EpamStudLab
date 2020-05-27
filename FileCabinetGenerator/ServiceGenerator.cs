using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Сontains methods for generating values for fields <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class ServiceGenerator
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly char[] arrayGender = { 'M', 'W' };
        private int randomYear;
        private int randomMonth;
        private int randomDay;
        private int index;

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        /// <param name ="record">Input record.</param>
        /// <returns>new new cloned object <see cref ="FileCabinetRecord"/>.</returns>
        public static FileCabinetRecord DeepCopy(FileCabinetRecord record)
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
        /// Generating random data for Gender and DateOfBirth.
        /// </summary>
        public void GeneratorGenderAndDateOfBirth()
        {
            Random rnd = new Random();
            this.index = rnd.Next(this.arrayGender.Length - 1);
            this.randomYear = rnd.Next(MinDate.Year, DateTime.Now.Year);
            this.randomMonth = rnd.Next(1, 12);
            this.randomDay = rnd.Next(1, DateTime.DaysInMonth(this.randomYear, this.randomMonth));
        }

        /// <summary>
        /// Generating random data in the interval.
        /// </summary>
        /// <param name="valueToStart">Start id.</param>
        /// <param name="amountOfGeneratedRecords">Count of generated records.</param>
        public void CreateRecordRandomValues(int valueToStart, int amountOfGeneratedRecords)
        {
            var lastIdRecord = amountOfGeneratedRecords + valueToStart;
            while (valueToStart < lastIdRecord)
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
        /// makes a snapshot of an list.
        /// </summary>
        /// <returns>new cloned object type of<see cref="FileCabinetServiceGeneratorSnapshot"/> as an array.</returns>
        public FileCabinetServiceGeneratorSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceGeneratorSnapshot(this.list.Select(x => DeepCopy(x)).ToArray());
        }
    }
}
