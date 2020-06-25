using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.TablePrinter;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for shows records.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Where = "where";
        private const string And = "and";
        private const string Or = "or";
        private const int BeforeWhere = 0;
        private const int AfterWhere = 1;
        private const string ParametersTemplateWithCondition = @"([a-zA-Z0-9_]{1,60}\,?\s){1,7}(\bwhere\b)\s([a-zA-Z0-9_]{1,60}\s?\=\s?\'[a-zA-Z0-9_/]{1,60}\'\,?)\s(and|or)\s([a-zA-Z0-9_]{1,60}\s?\=\s?\'[a-zA-Z0-9_/]{1,60}\'\,?)";
        private const string ParametersTemplateWithoutCondition = @"([a-zA-Z0-9_]{1,60}\,?\s?){1,7}";
        private const string InvalidFormat = "Invalid command format of 'select' command.";
        private const string FirstName = "FIRSTNAME";
        private const string LastName = "LASTNAME";
        private const string Gender = "GENDER";
        private const string DateOfBirth = "DATEOFBIRTH";
        private const string Id = "ID";
        private const string Salary = "SALARY";
        private const string Age = "AGE";
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileCabinetRecord record;
        private IEnumerable<FileCabinetRecord> records;
        private IEnumerable<FileCabinetRecord> recordsToShow;
        private IEnumerable<string> propertiesToShow;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input record.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "select";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Select(request.Parameters);
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

        private void Select(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(parameters);
                }

                var parametersArray = parameters.Split(Where, StringSplitOptions.None);

                if (parametersArray.Length == 1)
                {
                    if (!IsRightCommandSyntax(ParametersTemplateWithoutCondition, string.Join(string.Empty, parametersArray)))
                    {
                        throw new FormatException(InvalidFormat);
                    }

                    this.recordsToShow = this.service.GetRecords();
                }
                else if (!IsRightCommandSyntax(ParametersTemplateWithCondition, parameters))
                {
                    throw new FormatException(InvalidFormat);
                }

                this.propertiesToShow = parametersArray[BeforeWhere].Trim().Split(",").Select(prop => prop.Trim().ToUpperInvariant()).AsEnumerable();

                if (parametersArray.Length == 2)
                {
                    var isContainsAnd = ContainsAnd(parametersArray[AfterWhere], And);
                    var separators = new string[] { Or, And };
                    var parametersAfterWhere = parametersArray[AfterWhere].Trim().Split(' ');
                    var parametersAfterWhereWithoutSeparators = parametersAfterWhere.Select(item => separators
                    .Contains(item) ? item.Replace(item, ",", StringComparison.InvariantCultureIgnoreCase) : item).ToArray();

                    var keyValuePairs = GetKeyValuePairs(string.Join(string.Empty, parametersAfterWhereWithoutSeparators)).ToList();

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

                    this.recordsToShow = this.list.OrderBy(record => record.Id).AsEnumerable();
                }

                TableRecordDraw.PrintTable(this.recordsToShow, this.propertiesToShow);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
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
                    throw new FormatException("Invalid command format of 'select' command.");
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
                    throw new FormatException("Invalid command format of 'select' command.");
            }
        }
    }
}
