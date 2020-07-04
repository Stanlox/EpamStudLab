using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that is intended for processing user data and further processing it.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Bandaruk Maxim";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static bool isRunning = true;
        private static IRecordValidator defaultValidator = new ValidatorBuilder().CreateDefault();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(defaultValidator);
        private static FileStream fileStream;

        /// <summary>
        /// Point of entry.
        /// </summary>
        /// <param name="args">command line parameter.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args == null)
                {
                    throw new ArgumentNullException(nameof(args));
                }

                Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
                ParseArguments(args);
                Console.WriteLine(Program.HintMessage);
                Console.WriteLine();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }

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

        private static CommandHandlerBase CreateCommandHandlers(IFileCabinetService service)
        {
            var helpCommandHandler = new HelpCommandHandler();
            var createCommandHandler = new CreateCommandHandler(service);
            var exitCommandHandler = new ExitCommandHandler(ChangeRunning, fileStream);
            var exportCommandHandler = new ExportCommandHandler(service);
            var importCommandHandler = new ImportCommandHandler(service);
            var purgeCommandHandler = new PurgeCommandHandler(service);
            var statCommandHandler = new StatCommandHandler(service);
            var insertCommandHandler = new InsertCommandHandler(service);
            var deleteCommandHandler = new DeleteCommandHandler(service);
            var selectCommandHandler = new SelectCommandHandler(service);
            var updateCommandHandler = new UpdateCommandHandler(service);
            var printMissedCommandHandler = new PrintMissedCommandHandler();
            helpCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(selectCommandHandler);
            selectCommandHandler.SetNext(printMissedCommandHandler);
            return helpCommandHandler;
        }

        private static void ParseArguments(string[] arguments)
        {
            if (arguments.Length != 0)
            {
                var parameterCommandLine = string.Join(' ', arguments);
                var validationsRules = parameterCommandLine.Trim(' ').Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                var longDescriptionValidationsRules = "--validation-rules";
                var shortDescriptionValidationsRules = "-v";
                var longDescriptionUseTypeService = "--storage";
                var shortDescriptionUseTypeService = "-s";
                var useStopWatch = "-use-stopwatch";
                var useLogger = "-use-logger";
                string[] arrayCommandLine = { longDescriptionValidationsRules, shortDescriptionValidationsRules, longDescriptionUseTypeService, shortDescriptionUseTypeService, useStopWatch, useLogger };
                var arrayMatchingElements = validationsRules.Where(x => arrayCommandLine.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase))).ToArray();
                var isSrtingContainsStopWatchParameter = arrayMatchingElements.Contains(useStopWatch);
                var isStringContainsLoggerParameter = arrayMatchingElements.Contains(useLogger);

                for (int i = 0; i < arrayMatchingElements.Length; i++)
                {
                    switch (arrayMatchingElements[i])
                    {
                        case "--validation-rules":
                        case "-v":
                            var parameterValidation = "custom";
                            if (string.Equals(validationsRules[(2 * i) + 1], parameterValidation, StringComparison.OrdinalIgnoreCase))
                            {
                                fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateCustom());
                                Console.WriteLine("Using custom validation rules.");
                            }

                            break;
                        case "--storage":
                        case "-s":
                            var parameterService = "file";
                            if (string.Equals(validationsRules[(2 * i) + 1], parameterService, StringComparison.OrdinalIgnoreCase))
                            {
                                fileStream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                                fileCabinetService = new FileCabinetFilesystemService(fileStream);
                                Console.WriteLine("Using default validation rules.");
                            }

                            break;
                        default:
                            Console.WriteLine("Using default validation rules.");
                            break;
                    }
                }

                if (isSrtingContainsStopWatchParameter || isStringContainsLoggerParameter)
                {
                    if (isStringContainsLoggerParameter)
                    {
                        fileCabinetService = new ServiceLogger(fileCabinetService);
                    }
                    else
                    {
                        fileCabinetService = new ServiceMeter(fileCabinetService);
                    }
                }
            }
            else
            {
                Console.WriteLine("Using default validation rules.");
            }
        }
    }
}