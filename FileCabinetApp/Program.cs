using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that is intended for processing user data and further processing it.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Bandaruk Maxim";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static bool isCorrect = true;
        private static bool isRunning = true;
        private static DateTime dateValue;
        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static FileCabinetRecord[] listRecordsInService;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "view the number of records" },
            new string[] { "create", "create new user" },
            new string[] { "list", "view a list of records added to the service" },
            new string[] { "edit", "edit record" },
            new string[] { "find", "find record by a known value" },
        };

        /// <summary>
        /// Point of entry.
        /// </summary>
        public static void Main()
        {
            Console.Write("Validations rules: ");
            var validationsRules = Console.ReadLine().Trim(' ').Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            var longDescription = "--validation-rules";
            var shortDescription = "-v";
            if (validationsRules.Length == 0)
            {
                Console.WriteLine("Using default validation rules.");
                fileCabinetService.CreateValidator(new CustomValidator());
            }
            else if (string.Compare(validationsRules[0], longDescription, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(validationsRules[0], shortDescription, StringComparison.OrdinalIgnoreCase) == 0)
            {
                string parameter = "custom";
                if (string.Equals(validationsRules[1], parameter, StringComparison.OrdinalIgnoreCase))
                {
                    fileCabinetService.CreateValidator(new CustomValidator());
                    Console.WriteLine("Using custom validation rules.");
                }
                else
                {
                    fileCabinetService.CreateValidator(new CustomValidator());
                    Console.WriteLine("Using default validation rules.");
                }
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            string repeatIfDataIsNotCorrect = parameters;
            try
            {
                Program.UserData();
                Program.fileCabinetService.CreateRecord(fileCabinetServiceContext);
                Console.WriteLine($"Record # {Program.fileCabinetService.GetStat()} is created.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
            {
                Console.WriteLine(ex.Message);
                isCorrect = false;
            }
            finally
            {
                if (!isCorrect)
                {
                    Console.WriteLine("Your data is incorrect, please try again");
                    isCorrect = true;
                    Create(repeatIfDataIsNotCorrect);
                }
            }
        }

        private static void List(string parameters)
        {
            listRecordsInService = Program.fileCabinetService.GetRecords();
            ListRecord(listRecordsInService);
        }

        private static void Edit(string parameters)
        {
            try
            {
                int getNumberEditRecord = int.Parse(parameters, CultureInfo.CurrentCulture);
                if (getNumberEditRecord > Program.fileCabinetService.GetStat())
                {
                    throw new ArgumentException($"#{parameters} record in not found. ");
                }

                Program.UserData();
                Program.fileCabinetService.EditRecord(getNumberEditRecord, fileCabinetServiceContext);
                Console.WriteLine($"Record #{parameters} is updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Find(string parameters)
        {
            try
            {
                string parameterValue = parameters.Split(' ').Last().Trim('"');
                string[] parameterArray = parameters.Split(' ');
                var parameterName = parameterArray[parameterArray.Length - 2];
                switch (parameterName.ToLower(CultureInfo.CurrentCulture))
                {
                    case "firstname":
                        listRecordsInService = Program.fileCabinetService.FindByFirstName(parameterValue);
                        ListRecord(listRecordsInService);
                        break;
                    case "lastname":
                        listRecordsInService = Program.fileCabinetService.FindByLastName(parameterValue);
                        ListRecord(listRecordsInService);
                        break;
                    case "dateofbirth":
                        listRecordsInService = Program.fileCabinetService.FindByDateOfBirth(parameterValue);
                        ListRecord(listRecordsInService);
                        break;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ListRecord(FileCabinetRecord[] listRecordsInService)
        {
            for (int i = 0; i < listRecordsInService.Length; i++)
            {
                var builder = new StringBuilder();
                builder.Append($"{listRecordsInService[i].Id}, ");
                builder.Append($"{listRecordsInService[i].FirstName}, ");
                builder.Append($"{listRecordsInService[i].LastName}, ");
                builder.Append($"{listRecordsInService[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{listRecordsInService[i].Gender}, ");
                builder.Append($"{listRecordsInService[i].Age}, ");
                builder.Append($"{listRecordsInService[i].Salary}");
                Console.WriteLine("#" + builder.ToString());
            }
        }

        private static void UserData()
        {
            Console.Write("First name: ");
            fileCabinetServiceContext.FirstName = Console.ReadLine();
            Console.Write("Last Name: ");
            fileCabinetServiceContext.LastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            string dateOfBirth = Console.ReadLine();
            Console.Write("Gender (M/W): ");
            fileCabinetServiceContext.Gender = char.Parse(Console.ReadLine());
            Console.Write("Age: ");
            fileCabinetServiceContext.Age = short.Parse(Console.ReadLine(), CultureInfo.CurrentCulture);
            Console.Write("Salary: ");
            fileCabinetServiceContext.Salary = decimal.Parse(Console.ReadLine(), CultureInfo.CurrentCulture);
            var parsed = DateTime.TryParse(dateOfBirth, out dateValue);
            if (!parsed)
            {
                throw new ArgumentException($"{nameof(dateOfBirth)}");
            }

            fileCabinetServiceContext.DateOfBirth = dateValue;
        }
    }
}