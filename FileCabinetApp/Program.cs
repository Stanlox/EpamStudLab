using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Bandaruk Maxim";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "view the number of records" },
            new string[] { "create", "create new user" },
            new string[] { "list", "view a list of records added to the service" },
        };

        public static void Main()
        {
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
            try
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                Console.Write("Date of birth: ");
                string dataOfBirth = Console.ReadLine();
                Console.Write("Gender (M/W): ");
                char gender = char.Parse(Console.ReadLine());
                Console.Write("Age: ");
                short age = short.Parse(Console.ReadLine(), CultureInfo.CurrentCulture);
                Console.Write("Salary: ");
                decimal salary = decimal.Parse(Console.ReadLine(), CultureInfo.CurrentCulture);
                DateTime dateValue;
                var parsed = DateTime.TryParse(dataOfBirth, out dateValue);
                if (!parsed)
                {
                    throw new ArgumentException($"{nameof(parsed)}");
                }
                else
                {
                    Program.fileCabinetService.CreateRecord(firstName, lastName, dateValue, gender, age, salary);
                    Console.WriteLine($"Record # {Program.fileCabinetService.GetStat()} is created.");
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (OverflowException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void List(string parameters)
        {
            var listRecordsInService = Program.fileCabinetService.GetRecords();
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
    }
}