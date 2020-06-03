using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that is intended for processing user data and further processing it.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Bandaruk Maxim";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        public const int CommandHelpIndex = 0;
        public const int DescriptionHelpIndex = 1;
        public const int ExplanationHelpIndex = 2;
        public static bool isCorrect = true;
        private static bool isRunning = true;
        public static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        public static ReadOnlyCollection<FileCabinetRecord> listRecordsInService;
        public static FileStream fileStream;
        public static FileCabinetServiceSnapshot snapshot; 

        public static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "view the number of records." },
            new string[] { "create", "create new user." },
            new string[] { "list", "view a list of records added to the service." },
            new string[] { "edit", "edit record." },
            new string[] { "find", "find record by a known value." },
            new string[] { "export ", "export data to a file in format csv or xml." },
            new string[] { "import", "import data from a file." },
            new string[] { "remove", "remove record by id." },
            new string[] { "purge", "deleting \"voids\" in the data file.", },
        };

        /// <summary>
        /// Point of entry.
        /// </summary>
        /// <param name="args">command line parameter.</param>
        public static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                var parameterCommandLine = string.Join(' ', args);
                var validationsRules = parameterCommandLine.Trim(' ').Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                var longDescriptionValidationsRules = "--validation-rules";
                var shortDescriptionValidationsRules = "-v";
                var longDescriptionUseTypeService = "--storage";
                var shortDescriptionUseTypeService = "-s";
                string[] arrayCommandLine = { longDescriptionValidationsRules, shortDescriptionValidationsRules, longDescriptionUseTypeService, shortDescriptionUseTypeService };
                var arrayMatchingElements = validationsRules.Where(x => arrayCommandLine.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase))).ToArray();

                for (int i = 0; i < arrayMatchingElements.Length; i++)
                {
                    if (string.Compare(arrayMatchingElements[i], longDescriptionValidationsRules, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(arrayMatchingElements[i], shortDescriptionValidationsRules, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var parameter = "custom";
                        if (string.Equals(validationsRules[(2 * i) + 1], parameter, StringComparison.OrdinalIgnoreCase))
                        {
                            fileCabinetService = new FileCabinetMemoryService(new CustomValidator());
                            Console.WriteLine("Using custom validation rules.");
                        }
                        else
                        {
                            Console.WriteLine("Using default validation rules.");
                        }
                    }

                    if (string.Compare(arrayMatchingElements[i], longDescriptionUseTypeService, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(arrayMatchingElements[i], shortDescriptionUseTypeService, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var parameter = "file";
                        if (string.Equals(validationsRules[(2 * i) + 1], parameter, StringComparison.OrdinalIgnoreCase))
                        {
                            fileStream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                            fileCabinetService = new FileCabinetFilesystemService(fileStream);
                        }
                    }
                }

                if (!(arrayMatchingElements.Contains(longDescriptionValidationsRules) || arrayMatchingElements.Contains(shortDescriptionValidationsRules)) || arrayMatchingElements.Length == 0)
                {
                    Console.WriteLine("Using default validation rules.");
                }
            }
            else
            {
                Console.WriteLine("Using default validation rules.");
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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                var commandHandler = CreateCommandHandlers(Program.fileCabinetService);
                commandHandler.Handle(
                        new AppCommandRequest
                        {
                            Command = command,
                            Parameters = parameters,
                        });
            }
            while (isRunning);
        }

        private static void ChangeRunning(bool isRun)
        {
            isRunning = isRun;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                var builder = new StringBuilder();
                builder.Append($"{item.Id}, ");
                builder.Append($"{item.FirstName}, ");
                builder.Append($"{item.LastName}, ");
                builder.Append($"{item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{item.Gender}, ");
                builder.Append($"{item.Age}, ");
                builder.Append($"{item.Salary}");
                Console.WriteLine("#" + builder.ToString());
            }
        }

        private static CommandHandlerBase CreateCommandHandlers(IFileCabinetService service)
        {
            var listCommandHandler = new ListCommandHandler(Program.fileCabinetService, DefaultRecordPrint);
            var findCommandHandler = new FindCommandHandler(Program.fileCabinetService, DefaultRecordPrint);
            var helpCommandHandler = new HelpCommandHandler();
            var createCommandHandler = new CreateCommandHandler(service);
            var editCommandHandler = new EditCommandHandler(service);
            var exitCommandHandler = new ExitCommandHandler(ChangeRunning);
            var exportCommandHandler = new ExportCommandHandler(service);
            var importCommandHandler = new ImportCommandHandler(service);
            var purgeCommandHandler = new PurgeCommandHandler(service);
            var removeCommandHandler = new RemoveCommandHandler(service);
            var statCommandHandler = new StatCommandHandler(service);
            var printMissedCommandHandler = new PrintMissedCommandHandler();
            helpCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(listCommandHandler);
            listCommandHandler.SetNext(printMissedCommandHandler);
            return helpCommandHandler;
        }
    }
}