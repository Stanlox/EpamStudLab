using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
            GuardClauses.IsNullOrEmpty(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckLength(firstName, nameof(firstName), lastName, nameof(lastName));
            GuardClauses.CheckDateRange(dateOfBirth, nameof(dateOfBirth));
            GuardClauses.CheckGender(gender, nameof(gender));
            GuardClauses.CheckSalarySign(salary, nameof(salary));
            GuardClauses.CheckAge(age, nameof(age));

            var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    Age = age,
                    Salary = salary,
                };
            this.list.Add(record);
            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] array = new FileCabinetRecord[this.list.Count];
            array = this.list.ToArray();
            return array;
        }

        public int GetStat()
        {
           return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short age, decimal salary)
        {
                foreach (var record in this.list.Where(x => x.Id == id))
                {
                    record.Id = id;
                    record.FirstName = firstName;
                    record.LastName = lastName;
                    record.DateOfBirth = dateOfBirth;
                    record.Gender = gender;
                    record.Age = age;
                    record.Salary = salary;
                }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            this.list = this.list.Where(x => string.Equals(x.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            FileCabinetRecord[] array = this.GetRecords();
            return array;
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            this.list = this.list.Where(x => string.Equals(x.LastName, lastName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            FileCabinetRecord[] array = this.GetRecords();
            return array;
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime birthDate = DateTime.Parse(dateOfBirth, CultureInfo.CurrentCulture);
            Console.WriteLine(birthDate);
            this.list = this.list.Where(x => DateTime.Equals(x.DateOfBirth, birthDate)).ToList();
            FileCabinetRecord[] array = this.GetRecords();
            return array;
        }
    }
}
