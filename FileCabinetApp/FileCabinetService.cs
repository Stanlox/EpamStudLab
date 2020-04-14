using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

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
    }
}
