using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for delete a record.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Separator = "where";
        private const string ParametersTemplate = @".([a-zA-Z0-9_]{1,10}\s?\=\s?\'[a-zA-Z0-9_]{1,10}\'\,?\s?(\bor\b)?(\band\b)?\s?){1,10}";
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string Gender = "gender";
        private const string DateOfBirth = "dateofbirth";
        private const string Id = "id";
        private const string Salary = "salary";
        private const string Age = "age";
        private const string Or = "or";
        private const string And = "and";
        private readonly StringBuilder builder = new StringBuilder();
        private IEnumerable<FileCabinetRecord> records;
        private FileCabinetRecord record;
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "delete";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Delete(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static bool IsRightCommandSyntax(string template, string parameters)
        {
            return Regex.IsMatch(parameters, template);
        }

        private static bool IsParse(bool isParce, object parameter)
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

        private void Delete(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new FormatException("Invalid command format of 'delete' command.");
                }

                if (parameters.Split()[0].ToString(CultureInfo.InvariantCulture) != Separator)
                {
                    throw new FormatException("Invalid command format of 'delete' command.");
                }

                var pairsOfParameterValue = parameters.Trim().Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var request = string.Join(string.Empty, pairsOfParameterValue);

                if (!IsRightCommandSyntax(ParametersTemplate, request))
                {
                    throw new FormatException("Invalid command format of 'delete' command.");
                }

                this.FindRecord(request);

                if (this.list.Count == 0)
                {
                    throw new ArgumentException("Record(s) with these parameters was found");
                }

                this.Print();
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Invalid command format of 'delete' command.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Print()
        {
            var sortedRecords = this.list.OrderBy(record => record.Id).ToList();
            for (int j = 0; j < sortedRecords.Count; j++)
            {
                this.service.RemoveRecord(sortedRecords[j].Id);
                if (j + 1 == sortedRecords.Count)
                {
                    this.builder.Append($"# {sortedRecords[j].Id} ");
                }
                else
                {
                    this.builder.Append($"# {sortedRecords[j].Id}, ");
                }
            }

            if (sortedRecords.Count == 1)
            {
                Console.WriteLine($"Record {this.builder.ToString()} is deleted.");
            }
            else
            {
                Console.WriteLine($"Record {this.builder.ToString()} are deleted.");
            }
        }

        private void RequestConsistsAnd(string key, string value, string key2, string value2)
        {
            this.FindRecordByKey(key, value);
            this.FindRecordByKey(key2, value2);
            string[] keys = { key, key2 };
            string[] values = { value, value2 };
            bool isParse;
            for (int i = 0; i < keys.Length; i++)
            {
                switch (keys[i])
                {
                    case Id:

                        isParse = int.TryParse(values[i], out int id);
                        IsParse(isParse, id);
                        this.list = this.list.Where(record => record.Id == id).ToList();
                        break;
                    case FirstName:

                        this.list = this.list.Where(record => string.Equals(record.FirstName, values[i], StringComparison.CurrentCultureIgnoreCase)).ToList();
                        break;
                    case LastName:

                        this.list = this.list.Where(record => string.Equals(record.LastName, values[i], StringComparison.CurrentCultureIgnoreCase)).ToList();
                        break;
                    case DateOfBirth:

                        isParse = DateTime.TryParse(values[i], out DateTime date);
                        IsParse(isParse, date);
                        this.list = this.list.Where(record => record.DateOfBirth == date).ToList();
                        break;
                    case Salary:

                        isParse = decimal.TryParse(values[i], out decimal salary);
                        IsParse(isParse, salary);
                        this.list = this.list.Where(record => record.Salary == salary).ToList();
                        break;
                    case Gender:

                        isParse = char.TryParse(values[i], out char gender);
                        IsParse(isParse, gender);
                        this.list = this.list.Where(record => string.Equals(record.Gender.ToString(CultureInfo.InvariantCulture), values[i], StringComparison.CurrentCultureIgnoreCase)).ToList();
                        break;
                    case Age:

                        isParse = short.TryParse(values[i], out short age);
                        IsParse(isParse, age);
                        this.list = this.list.Where(record => record.Age == age).ToList();
                        break;
                    default:
                        throw new FormatException("Invalid command format of 'delete' command.");
                }
            }
        }

        private void AddRecord(List<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
               if (!this.list.Exists(record => record.Id == item.Id))
               {
                    this.list.Add(item);
               }
            }
        }

        private void FindRecord(string request)
        {
            var pairsOfParameterValue = request.
                Replace("=", string.Empty, StringComparison.OrdinalIgnoreCase).
                Trim();

            string[] pairsOfParameterValueArray = pairsOfParameterValue
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim('\'')).ToArray();

            if (Array.Exists(pairsOfParameterValueArray, elements => elements == And))
            {
                var index = 0;
                for (int i = 0; i < pairsOfParameterValue.Length; i++)
                {
                    if (pairsOfParameterValueArray[i] == And)
                    {
                        index = i;
                        break;
                    }
                }

                var key = pairsOfParameterValueArray[index - 2];
                var value = pairsOfParameterValueArray[index - 1];
                var key2 = pairsOfParameterValueArray[index + 1];
                var value2 = pairsOfParameterValueArray[index + 2];
                this.RequestConsistsAnd(key, value, key2, value2);
                return;
            }

            for (int i = 0; i < pairsOfParameterValueArray.Length; i++)
            {
                if (pairsOfParameterValueArray.Length - 1 == i)
                {
                    break;
                }

                var parameterKey = pairsOfParameterValueArray[i];
                var parameterValue = pairsOfParameterValueArray[i + 1];
                if (string.Equals(pairsOfParameterValueArray[i + 1], Or, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(pairsOfParameterValueArray[i + 1], And, StringComparison.OrdinalIgnoreCase))
                {
                    parameterValue = pairsOfParameterValueArray[i + 3];
                    parameterKey = pairsOfParameterValueArray[i + 2];
                }

                if (i + 3 == pairsOfParameterValueArray.Length)
                {
                    break;
                }

                this.FindRecordByKey(parameterKey, parameterValue);
            }
        }

        private void FindRecordByKey(string parameterKey, string parameterValue)
        {
            bool isParse;
            switch (parameterKey)
            {
                case Id:

                    isParse = int.TryParse(parameterValue, out int id);
                    IsParse(isParse, id);
                    this.record = this.service.FindById(id);
                    this.list.Add(this.record);
                    break;
                case FirstName:

                    this.records = this.service.FindByFirstName(parameterValue);
                    this.AddRecord(this.records.ToList());
                    break;
                case LastName:

                    this.records = this.service.FindByLastName(parameterValue);
                    this.AddRecord(this.records.ToList());
                    break;
                case DateOfBirth:

                    isParse = DateTime.TryParse(parameterValue, out DateTime date);
                    IsParse(isParse, date);
                    this.records = this.service.FindByDateOfBirth(date);
                    this.AddRecord(this.records.ToList());
                    break;
                case Salary:

                    isParse = decimal.TryParse(parameterValue, out decimal salary);
                    IsParse(isParse, salary);
                    this.records = this.service.FindBySalary(salary);
                    this.AddRecord(this.records.ToList());
                    break;
                case Gender:

                    isParse = char.TryParse(parameterValue, out char gender);
                    IsParse(isParse, gender);
                    this.records = this.service.FindByGender(gender);
                    this.AddRecord(this.records.ToList());
                    break;
                case Age:

                    isParse = short.TryParse(parameterValue, out short age);
                    IsParse(isParse, age);
                    this.records = this.service.FindByAge(age);
                    this.AddRecord(this.records.ToList());
                    break;
                default:
                    throw new FormatException("Invalid command format of 'delete' command.");
            }
        }
    }
}
