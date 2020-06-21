using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for updating records.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string ParametersTemplate = @"(\bset\b)\s(\s?[a-zA-Z0-9_]{1,20}\s?\=\s?\'?[a-zA-Z0-9_/]{1,20}\'?\s?\,?\s?){1,10}\s(\bwhere\b)\s([a-zA-Z0-9_]{1,20}\s?\=\s?\'?[a-zA-Z0-9_/]{1,20}\'?\,?\s?(\bor\b)?(\band\b)?\s?){1,10}";
        private const string Separator = "where";
        private const string InvalidFormat = "Invalid command format of 'update' command.";
        private const int BeforeWhere = 0;
        private const int AfterWhere = 1;
        private const string FirstName = "FIRSTNAME";
        private const string LastName = "LASTNAME";
        private const string Gender = "GENDER";
        private const string DateOfBirth = "DATEOFBIRTH";
        private const string Id = "ID";
        private const string Salary = "SALARY";
        private const string Age = "AGE";
        private const string Or = "or";
        private const string And = "and";

        private readonly Tuple<string, Action<FileCabinetServiceContext, string>>[] commands = new Tuple<string, Action<FileCabinetServiceContext, string>>[]
          {
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("firstname", GetFirstName),
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("lastname", GetLastName),
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("dateofbirth", GetDateOfBirth),
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("age", GetAge),
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("salary", GetSalary),
                    new Tuple<string, Action<FileCabinetServiceContext, string>>("gender", GetGender),
          };

        private FileCabinetServiceContext recordContext = new FileCabinetServiceContext();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private IEnumerable<FileCabinetRecord> records;
        private FileCabinetRecord record;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "update";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Update(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void GetFirstName(FileCabinetServiceContext record, string parameters)
        {
            record.FirstName = parameters.Trim();
        }

        private static void GetLastName(FileCabinetServiceContext record, string parameters)
        {
            record.LastName = parameters.Trim();
        }

        private static void GetDateOfBirth(FileCabinetServiceContext record, string parameters)
        {
            var isConverted = DateTime.TryParse(parameters, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            IsParse(isConverted, dateOfBirth);
            record.DateOfBirth = dateOfBirth;
        }

        private static void GetAge(FileCabinetServiceContext record, string parameters)
        {
            var isConverted = short.TryParse(parameters, out short age);
            IsParse(isConverted, age);

            record.Age = age;
        }

        private static void GetSalary(FileCabinetServiceContext record, string parameters)
        {
            var isConverted = decimal.TryParse(parameters, out decimal salary);
            IsParse(isConverted, salary);

            record.Salary = salary;
        }

        private static void GetGender(FileCabinetServiceContext record, string parameters)
        {
            var isConverted = char.TryParse(parameters, out char gender);
            IsParse(isConverted, gender);

            record.Gender = gender;
        }

        private static bool IsRightCommandSyntax(string template, string parameters)
        {
            return Regex.IsMatch(parameters, template);
        }

        private static IEnumerable<(string key, string value)> GetKeyValuePairs(string source)
        {
            const int keyIndex = 0;
            const int valueIndex = 1;
            var result = new List<(string key, string value)>();
            var keyValuePairs = source.Split(",");
            foreach (var keyValuePair in keyValuePairs)
            {
                var keyValueArray = keyValuePair.Split('=');
                result.Add((keyValueArray[keyIndex].Trim(), keyValueArray[valueIndex].Trim('\'', ' ')));
            }

            return result;
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

        private static bool ContainsAnd(string parameters, string separators)
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

        private void Update(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(parameters);
                }

                if (!IsRightCommandSyntax(ParametersTemplate, parameters))
                {
                    throw new FormatException(InvalidFormat);
                }

                var parametersArray = parameters.Trim().Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var keyValuePairs = GetKeyValuePairs(parametersArray[BeforeWhere].Replace("set", string.Empty, StringComparison.OrdinalIgnoreCase).Trim());

                foreach (var pair in keyValuePairs)
                {
                    var index = Array.FindIndex(this.commands, j => j.Item1.Equals(pair.key, StringComparison.InvariantCultureIgnoreCase));
                    if (index >= 0)
                    {
                        this.commands[index].Item2(this.recordContext, pair.value);
                    }
                    else
                    {
                        throw new FormatException(InvalidFormat);
                    }
                }

                if (parametersArray[AfterWhere].Contains(Or, StringComparison.InvariantCultureIgnoreCase) || parametersArray[AfterWhere].Contains(And, StringComparison.InvariantCultureIgnoreCase))
                {
                    var separators = new string[] { Or, And };
                    var isContainsAnd = ContainsAnd(parametersArray[AfterWhere], And);
                    var parametersArrayAfterWhere = parametersArray[AfterWhere].Trim().Split(' ');
                    var countSeparators = 0;
                    for (int i = 0; i < parametersArrayAfterWhere.Length; i++)
                    {
                        for (int j = 0; j < separators.Length; j++)
                        {
                            if (parametersArrayAfterWhere[i] == separators[j])
                            {
                                countSeparators++;
                                parametersArrayAfterWhere[i] = ",";
                            }
                        }
                    }

                    if (countSeparators > 1)
                    {
                        throw new FormatException(InvalidFormat);
                    }

                    var valueAndProperty = string.Join(string.Empty, parametersArrayAfterWhere);
                    keyValuePairs = GetKeyValuePairs(valueAndProperty);
                    var parametersAfterWhere = string.Join(string.Empty, parametersArray[AfterWhere]).Trim();
                    foreach (var pair in keyValuePairs)
                    {
                        this.FindRecordByKey(pair.key, pair.value);
                    }

                    if (isContainsAnd)
                    {
                        foreach (var pair in keyValuePairs)
                        {
                            this.FindRecordByMatchingCriteria(pair.key, pair.value);
                        }
                    }
                }
                else
                {
                     keyValuePairs = GetKeyValuePairs(parametersArray[AfterWhere].Trim());
                     foreach (var pair in keyValuePairs)
                     {
                        this.FindRecordByKey(pair.key, pair.value);
                     }
                }

                if (this.list.Count == 0)
                {
                    Console.WriteLine("Records with the specified criteria do not exist");
                    return;
                }

                foreach (var record in this.list)
                {
                    record.Id = record.Id;
                    this.recordContext.FirstName = this.recordContext.FirstName ?? record.FirstName;
                    this.recordContext.LastName = this.recordContext.LastName ?? record.LastName;
                    if (this.recordContext.DateOfBirth == DateTime.MinValue)
                    {
                        this.recordContext.DateOfBirth = record.DateOfBirth;
                    }

                    if (this.recordContext.Salary == 0)
                    {
                        this.recordContext.Salary = record.Salary;
                    }

                    if (this.recordContext.Age == 0)
                    {
                        this.recordContext.Age = record.Age;
                    }

                    if (this.recordContext.Gender == 0)
                    {
                        this.recordContext.Gender = record.Gender;
                    }

                    this.service.EditRecord(record.Id, this.recordContext);
                }

                StringBuilder builder = new StringBuilder();
                var sortedRecords = this.list.OrderBy(record => record.Id).ToList();

                for (int j = 0; j < sortedRecords.Count; j++)
                {
                    if (j + 1 == sortedRecords.Count)
                    {
                        builder.Append($"# {sortedRecords[j].Id} ");
                    }
                    else
                    {
                        builder.Append($"# {sortedRecords[j].Id}, ");
                    }
                }

                if (sortedRecords.Count == 1)
                {
                    Console.WriteLine($"Record {builder.ToString()}is updated.");
                }
                else
                {
                    Console.WriteLine($"Records {builder.ToString()}are updated.");
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine(InvalidFormat);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(InvalidFormat);
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

        private void FindRecordByKey(string parameterKey, string parameterValue)
        {
            bool isParse;
            switch (parameterKey.ToUpperInvariant())
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
                    throw new FormatException("Invalid command format of 'update' command.");
            }
        }

        private void FindRecordByMatchingCriteria(string key, string value)
        {
            bool isParse;
            switch (key.ToUpperInvariant())
            {
                case Id:

                    isParse = int.TryParse(value, out int id);
                    IsParse(isParse, id);
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
                    IsParse(isParse, date);
                    this.list = this.list.Where(record => record.DateOfBirth == date).ToList();
                    break;
                case Salary:

                    isParse = decimal.TryParse(value, out decimal salary);
                    IsParse(isParse, salary);
                    this.list = this.list.Where(record => record.Salary == salary).ToList();
                    break;
                case Gender:

                    isParse = char.TryParse(value, out char gender);
                    IsParse(isParse, gender);
                    this.list = this.list.Where(record => string.Equals(record.Gender.ToString(CultureInfo.InvariantCulture), value, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case Age:

                    isParse = short.TryParse(value, out short age);
                    IsParse(isParse, age);
                    this.list = this.list.Where(record => record.Age == age).ToList();
                    break;
                default:
                    throw new FormatException("Invalid command format of 'delete' command.");
            }
        }
    }
}
