using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp.Search
{
    /// <summary>
    /// Find records in service.
    /// </summary>
    public class FindRecordInService
    {
        private const string FirstName = "FIRSTNAME";
        private const string LastName = "LASTNAME";
        private const string Gender = "GENDER";
        private const string DateOfBirth = "DATEOFBIRTH";
        private const string Id = "ID";
        private const string Salary = "SALARY";
        private const string Age = "AGE";
        private IFileCabinetService service;
        private IEnumerable<FileCabinetRecord> records;
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileCabinetRecord record;
        private string commandName;
        private List<FileCabinetRecord> casheList = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FindRecordInService"/> class.
        /// </summary>
        /// <param name="service">Input type of service.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="list">List passed by key.</param>
        public FindRecordInService(IFileCabinetService service, string commandName, ref List<FileCabinetRecord> list)
        {
            this.service = service;
            this.commandName = commandName;
            this.list = list;
        }

        /// <summary>
        /// Checks whether the request contains 'and'.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <param name="separators">Input separators.</param>
        /// <returns>Boolean value is contained in the query 'and'.</returns>
        #pragma warning disable CA1822
        public bool ContainsAnd(string parameters, string separators)
        {
            int countAnd = (parameters.Length - parameters.Replace(separators, string.Empty, StringComparison.OrdinalIgnoreCase).Length) / separators.Length;
            if (countAnd > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// finds records in dictionary by key.
        /// </summary>
        /// <param name="parameterKey">Input key.</param>
        /// <param name="parameterValue">Input value corresponding to the key.</param>
        /// <returns>List of found records of a specific type.</returns>
        public List<FileCabinetRecord> FindRecordByKey(string parameterKey, string parameterValue)
        {
            this.casheList.Clear();
            bool isParse;
            switch (parameterKey.ToUpperInvariant())
            {
                case Id:

                    isParse = int.TryParse(parameterValue, out int id);
                    this.IsParse(isParse, id);
                    this.record = this.service.FindById(id);
                    this.list.Add(this.record);
                    this.casheList.Add(this.record);
                    break;
                case FirstName:

                    this.records = this.service.FindByFirstName(parameterValue);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                case LastName:

                    this.records = this.service.FindByLastName(parameterValue);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                case DateOfBirth:

                    isParse = DateTime.TryParse(parameterValue, out DateTime date);
                    this.IsParse(isParse, date);
                    this.records = this.service.FindByDateOfBirth(date);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                case Salary:

                    isParse = decimal.TryParse(parameterValue, out decimal salary);
                    this.IsParse(isParse, salary);
                    this.records = this.service.FindBySalary(salary);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                case Gender:

                    isParse = char.TryParse(parameterValue, out char gender);
                    this.IsParse(isParse, gender);
                    this.records = this.service.FindByGender(gender);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                case Age:

                    isParse = short.TryParse(parameterValue, out short age);
                    this.IsParse(isParse, age);
                    this.records = this.service.FindByAge(age);
                    this.AddRecord(this.records);
                    this.casheList.AddRange(this.records);
                    break;
                default:
                    throw new FormatException($"Invalid command format of '{this.commandName}' command.");
            }

            return this.casheList;
        }

        /// <summary>
        /// Find record by matching criteria.
        /// </summary>
        /// <param name="key">Input key.</param>
        /// <param name="value">Input value.</param>
        public void FindRecordByMatchingCriteria(string key, string value)
        {
            bool isParse;
            switch (key.ToUpperInvariant())
            {
                case Id:

                    isParse = int.TryParse(value, out int id);
                    this.IsParse(isParse, id);
                    this.list = this.list.Where(record => record.Id == id).ToList();
                    break;
                case FirstName:

                    this.list = this.list.Where(record => string.Equals(record.FirstName, value, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case LastName:
                    this.list = this.list.Where(record => string.Equals(record.LastName, value, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case DateOfBirth:

                    isParse = DateTime.TryParse(value, out DateTime date);
                    this.IsParse(isParse, date);
                    this.list = this.list.Where(record => record.DateOfBirth == date).ToList();
                    break;
                case Salary:

                    isParse = decimal.TryParse(value, out decimal salary);
                    this.IsParse(isParse, salary);
                    this.list = this.list.Where(record => record.Salary == salary).ToList();
                    break;
                case Gender:

                    isParse = char.TryParse(value, out char gender);
                    this.IsParse(isParse, gender);
                    this.list = this.list.Where(record => string.Equals(record.Gender.ToString(CultureInfo.InvariantCulture), value, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case Age:

                    isParse = short.TryParse(value, out short age);
                    this.IsParse(isParse, age);
                    this.list = this.list.Where(record => record.Age == age).ToList();
                    break;
                default:
                    throw new FormatException($"Invalid command format of '{this.commandName}' command.");
            }
        }

        private void AddRecord(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                if (!this.list.Exists(record => record.Id == item.Id))
                {
                    this.list.Add(item);
                }
            }
        }

        private bool IsParse(bool isParce, object parameter)
        {
            if (!isParce)
            {
                throw new FormatException($"The {parameter} format is incorrect");
            }
            else
            {
                return true;
            }
        }
    }
}
