using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Search;

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
        private const string Or = "or";
        private const string And = "and";
        private const string NameCommand = "update";

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
        private FindRecordInService find;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.find = new FindRecordInService(service, NameCommand, ref this.list);
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
                if (this.service is FileCabinetMemoryService)
                {
                    Caсhe.ClearCashe();
                }

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
                    var isContainsAnd = this.find.ContainsAnd(parametersArray[AfterWhere], And);
                    var parametersArrayAfterWhere = parametersArray[AfterWhere].Trim().Split(' ');
                    var countSeparators = 0;

                    var splitArrayAfterParameters = parametersArrayAfterWhere
                    .Select(item => separators
                    .Contains(item) ? item.Replace(item, ",", StringComparison.InvariantCultureIgnoreCase) : item).ToArray();

                    for (int i = 0; i < parametersArrayAfterWhere.Length; i++)
                    {
                        if (parametersArrayAfterWhere[i] != splitArrayAfterParameters[i])
                        {
                            countSeparators++;
                        }
                    }

                    if (countSeparators > 1)
                    {
                        throw new FormatException(InvalidFormat);
                    }

                    var valueAndProperty = string.Join(string.Empty, splitArrayAfterParameters);
                    keyValuePairs = GetKeyValuePairs(valueAndProperty);
                    var parametersAfterWhere = string.Join(string.Empty, parametersArray[AfterWhere]).Trim();
                    foreach (var pair in keyValuePairs)
                    {
                        this.find.FindRecordByKey(pair.key, pair.value);
                    }

                    if (isContainsAnd)
                    {
                        foreach (var pair in keyValuePairs)
                        {
                            this.find.FindRecordByMatchingCriteria(pair.key, pair.value);
                        }
                    }
                }
                else
                {
                     keyValuePairs = GetKeyValuePairs(parametersArray[AfterWhere].Trim());
                     foreach (var pair in keyValuePairs)
                     {
                        this.find.FindRecordByKey(pair.key, pair.value);
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
    }
}
