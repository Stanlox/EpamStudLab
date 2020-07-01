using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FileCabinetApp.Search;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for inserts records.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string ParametersTemplate = @"\(((.+,){6}.+)\) values \(((.+,){6}.+)\)";
        private const int PropertyNamesIndex = 1;
        private const int PropertyValuesIndex = 3;
        private static FileCabinetServiceContext record = new FileCabinetServiceContext();
        private int countCalledCommands = 0;

        private Tuple<string, Action<FileCabinetServiceContext, string>>[] commands = new Tuple<string, Action<FileCabinetServiceContext, string>>[]
            {
                new Tuple<string, Action<FileCabinetServiceContext, string>>("firstname", GetFirstName),
                new Tuple<string, Action<FileCabinetServiceContext, string>>("lastname", GetLastName),
                new Tuple<string, Action<FileCabinetServiceContext, string>>("dateofbirth", GetDateOfBirth),
                new Tuple<string, Action<FileCabinetServiceContext, string>>("salary", GetSalary),
                new Tuple<string, Action<FileCabinetServiceContext, string>>("age", GetAge),
                new Tuple<string, Action<FileCabinetServiceContext, string>>("gender", GetGender),
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public InsertCommandHandler(IFileCabinetService service)
        : base(service)
        {
        }

        /// <summary>
        /// Get first name from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input firstname parameter.</param>
        public static void GetFirstName(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            record.FirstName = parameters;
        }

        /// <summary>
        /// Get last name from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input last name parameter.</param>
        public static void GetLastName(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            record.LastName = parameters;
        }

        /// <summary>
        /// Get date of birth from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input date of birth parameter.</param>
        public static void GetDateOfBirth(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var isConverted = DateTime.TryParseExact(parameters, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            if (!isConverted)
            {
                throw new FormatException("Wrong Date of Birth format.");
            }

            record.DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Get age from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input age parameter.</param>
        public static void GetAge(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var isConverted = short.TryParse(parameters, out short age);
            if (!isConverted)
            {
                throw new FormatException("Wrong Age format.");
            }

            record.Age = age;
        }

        /// <summary>
        /// Get salary from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input salary parameter.</param>
        public static void GetSalary(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var isConverted = decimal.TryParse(parameters, out decimal salary);
            if (!isConverted)
            {
                throw new FormatException("Wrong Salary format.");
            }

            record.Salary = salary;
        }

        /// <summary>
        /// Get gender from string.
        /// </summary>
        /// <param name="record">Input FileCabinetServiceContext object.</param>
        /// <param name="parameters">Input gender parameter.</param>
        public static void GetGender(FileCabinetServiceContext record, string parameters)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var isConverted = char.TryParse(parameters, out char gender);
            if (!isConverted)
            {
                throw new FormatException("Wrong Gender format.");
            }

            record.Gender = gender;
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

            const string name = "insert";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                if (this.service is FileCabinetMemoryService)
                {
                    Caсhe.ClearCashe();
                }

                this.Insert(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static Tuple<string, string> GetPropertyNamesAndValues(string template, string parameters)
        {
            var regex = new Regex(template);
            var match = regex.Match(parameters);
            return Tuple.Create(match.Groups[PropertyNamesIndex].Value, match.Groups[PropertyValuesIndex].Value);
        }

        private static bool IsRightCommandSyntax(string template, string parameters)
        {
            return Regex.IsMatch(parameters, template);
        }

        /// <summary>
        /// Get id from string.
        /// </summary>
        /// <param name="parameters">Input id parameter.</param>
        private void GetId(string parameters)
        {
            var isConverted = int.TryParse(parameters.Trim(), out int id);
            if (!isConverted)
            {
                throw new FormatException("Wrong id format.");
            }

            var record = this.service.FindById(id);

            if (record != null)
            {
                throw new FormatException("Record with this id already exists");
            }
        }

        private void Insert(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters) || !IsRightCommandSyntax(ParametersTemplate, parameters))
                {
                    throw new FormatException("Invalid command format of 'insert' command.");
                }

                (var propertyNames, var propertyValues) = GetPropertyNamesAndValues(ParametersTemplate, parameters);
                var arrayPropertyNames = propertyNames.Split(',');
                var arrayPropertyValues = propertyValues.Split(',').Select(value => value.Trim().Trim('\'')).ToArray();
                if (arrayPropertyNames.Length != arrayPropertyValues.Length)
                {
                    throw new FormatException("Invalid command format of 'insert' command.");
                }

                for (var i = 0; i < arrayPropertyNames.Length; i++)
                {
                    var index = Array.FindIndex(this.commands, j => j.Item1.Equals(arrayPropertyNames[i].Trim(), StringComparison.InvariantCultureIgnoreCase));

                    if (index >= 0)
                    {
                        this.commands[index].Item2(record, arrayPropertyValues[i]);
                        this.countCalledCommands++;
                    }
                    else if (string.Equals(arrayPropertyNames[i], "id", StringComparison.OrdinalIgnoreCase))
                    {
                        this.GetId(arrayPropertyValues[i]);
                    }
                    else
                    {
                        throw new FormatException("Invalid command format of 'insert' command.");
                    }
                }

                if (this.countCalledCommands != this.commands.Length)
                {
                    throw new FormatException("Invalid command format of 'insert' command.");
                }

                var recordId = this.service.CreateRecord(record);
                Console.WriteLine($"Record #{recordId} is created.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
