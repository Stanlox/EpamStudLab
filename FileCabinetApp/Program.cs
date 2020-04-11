using System;
using System.Collections.Generic;
using System.Globalization;

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

        public static void Main(string[] args)
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
            bool isCorrectDataBirth = true;
            Console.Write("First name: ");
            string firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            string dataOfBirth = Console.ReadLine();
            DateTime dateValue;
            if (DateTime.TryParse(dataOfBirth, out dateValue))
            {
               Program.fileCabinetService.CreateRecord(firstName, lastName, dateValue);
               Console.WriteLine($"Record # {Program.fileCabinetService.GetStat()} is created.");
            }
            else
            {
                do
                {
                    Console.WriteLine("Please, Input correct Data of Birth in format DD/MM/YYYY ");
                    Console.Write("Date of birth: ");
                    dataOfBirth = Console.ReadLine();
                    if (DateTime.TryParse(dataOfBirth, out dateValue))
                    {
                        Program.fileCabinetService.CreateRecord(firstName, lastName, dateValue);
                        Console.WriteLine($"Record #{Program.fileCabinetService.GetStat()} is created.");
                        isCorrectDataBirth = false;
                    }
                }
                while (isCorrectDataBirth);
            }
        }

        private static void List(string parameters)
        {
            int sequentialNumberInlist = 1;
            var listRecordsInService = Program.fileCabinetService.GetRecords();
            for (int i = 0; i < listRecordsInService.Length; i++)
            {
                Console.Write("#" + sequentialNumberInlist + ", " + listRecordsInService[i].FirstName + ", " + listRecordsInService[i].LastName + ", " + listRecordsInService[i].DateOfBirth.ToString("D", CultureInfo.CurrentCulture));
                sequentialNumberInlist++;
                Console.WriteLine();
            }
        }
    }
}