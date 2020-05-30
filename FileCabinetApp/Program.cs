using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private static ReadOnlyCollection<FileCabinetRecord> listRecordsInService;
        private static FileStream fileStream;
        private static FileCabinetServiceSnapshot snapshot;
        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        private static string[][] helpMessages = new string[][]
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
            if (fileStream != null)
            {
                fileStream.Close();
            }

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
                listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < listRecordsInService.Count; i++)
                {
                    if (!listRecordsInService.Any(x => x.Id == getNumberEditRecord))
                    {
                        throw new ArgumentException($"#{getNumberEditRecord} record in not found. ");
                    }
                    else if (getNumberEditRecord == listRecordsInService[i].Id)
                    {
                        Program.UserData();
                        Program.fileCabinetService.EditRecord(getNumberEditRecord, fileCabinetServiceContext);
                        Console.WriteLine($"Record #{getNumberEditRecord} is updated.");
                        break;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Find(string parameters)
        {
            try
            {
                var parameterValue = parameters.Split(' ').Last().Trim('"');
                var parameterArray = parameters.Split(' ');
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

        private static void Export(string parameters)
        {
            bool rewrite = false;
            var noRewrite = 'n';
            const string xml = "xml";
            const string csv = "csv";
            try
            {
                snapshot = fileCabinetService.MakeSnapshot();
                var parameterArray = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fullPath = parameterArray.Last();
                var nameFile = Path.GetFileName(fullPath);
                var typeFile = parameterArray.First();
                if (File.Exists(nameFile))
                {
                    Console.Write($"File is exist - rewrite {nameFile}?[Y / n] ");
                    var rewriteOrNo = ReadInput(RewriteConverter, RewriteValidator);
                    char.ToLower(rewriteOrNo, CultureInfo.InvariantCulture);
                    if (char.Equals(rewriteOrNo, noRewrite))
                    {
                        rewrite = true;
                    }
                }

                try
                {
                    if (string.Equals(csv, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var sw = new StreamWriter(nameFile, rewrite))
                        {
                            snapshot.SaveToCsv(sw);
                            Console.WriteLine($"All records are exported to file {nameFile}");
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var sw = new StreamWriter(nameFile, rewrite))
                        {
                            snapshot.SaveToXml(sw);
                            Console.WriteLine($"All records are exported to file {nameFile}");
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Export failed: can't open file {fullPath}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Enter the file extension and his name or path");
            }
        }

        private static void Import(string parameters)
        {
            const string xml = "xml";
            const string csv = "csv";
            try
            {
                var parameterArray = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fullPath = parameterArray.Last();
                var nameFile = Path.GetFileName(fullPath);
                var typeFile = parameterArray.First();
                if (File.Exists(nameFile))
                {
                    snapshot = fileCabinetService.MakeSnapshot();
                    if (string.Equals(csv, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            snapshot.LoadFromCsv(fileStream);
                            Console.WriteLine($"{snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            fileCabinetService.Restore(snapshot);
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            snapshot.LoadFromXml(fileStream);
                            Console.WriteLine($"{snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            fileCabinetService.Restore(snapshot);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Import error: {fullPath} is not exist.");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Enter the file extension and his name or path");
            }
        }

        private static void Remove(string parameters)
        {
            try
            {
                int recordId;
                bool success = int.TryParse(parameters, out recordId);
                if (!success)
                {
                    Console.WriteLine("Conversion error.");
                }

                listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < listRecordsInService.Count; i++)
                {
                    if (!listRecordsInService.Any(x => x.Id == recordId))
                    {
                        throw new ArgumentException($"Record #{recordId} doesn't exists.");
                    }
                    else if (recordId == listRecordsInService[i].Id)
                    {
                        fileCabinetService.RemoveRecord(recordId);
                        Console.WriteLine($"Record #{recordId} is removed");
                        break;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Purge(string parameters)
        {
            try
            {
                var tuple = fileCabinetService.PurgeRecord();
                Console.WriteLine($"Data file processing is completed: {tuple.Item1} of {tuple.Item2} records were purged.");
            }
            catch (NotImplementedException)
            {
            }
        }

        private static void ListRecord(ReadOnlyCollection<FileCabinetRecord> listRecordsInService)
        {
            for (int i = 0; i < listRecordsInService.Count; i++)
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
            fileCabinetServiceContext.FirstName = ReadInput(StringConverter, FirstNameValidator);
            Console.Write("Last Name: ");
            fileCabinetServiceContext.LastName = ReadInput(StringConverter, LastNameValidator);
            Console.Write("Date of birth: ");
            fileCabinetServiceContext.DateOfBirth = ReadInput(DateOfBirthConverter, DateOfBirthValidator);
            Console.Write("Gender (M/W): ");
            fileCabinetServiceContext.Gender = ReadInput(GenderConverter, GenderValidator);
            Console.Write("Age: ");
            fileCabinetServiceContext.Age = ReadInput(AgeConverter, AgeValidator);
            Console.Write("Salary: ");
            fileCabinetServiceContext.Salary = ReadInput(SalaryConverter, SalaryValidator);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string> FirstNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input FirstName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string, string> StringConverter(string val)
        {
           return new Tuple<bool, string, string>(true, val, val);
        }

        private static Tuple<bool, string> LastNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input LastName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime val)
        {
            if (val == DateTime.MinValue)
            {
                return new Tuple<bool, string>(false, "Сheck input DateOfBirth");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, DateTime> DateOfBirthConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, DateTime>(false, "Empty field", DateTime.MinValue);
            }

            if (DateTime.TryParse(val, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(false, val, result);
            }
        }

        private static Tuple<bool, string> GenderValidator(char val)
        {
            if (val == char.MaxValue)
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, char> GenderConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, char>(false, "Empty field", char.MinValue);
            }

            if (char.TryParse(val, out char result))
            {
                return new Tuple<bool, string, char>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, char>(false, "The symbol is not of the char type", char.MinValue);
            }
        }

        private static Tuple<bool, string> AgeValidator(short val)
        {
            if (val <= 0)
            {
                return new Tuple<bool, string>(false, "Age can't be negative or equal 0");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, short> AgeConverter(string val)
        {
            if (short.TryParse(val, out short result))
            {
                return new Tuple<bool, string, short>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, short>(false, val, 0);
            }
        }

        private static Tuple<bool, string, decimal> SalaryConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, decimal>(false, "Empty string", 0);
            }
            else if (decimal.TryParse(val, out decimal result))
            {
                return new Tuple<bool, string, decimal>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, decimal>(false, val, 0);
            }
        }

        private static Tuple<bool, string> SalaryValidator(decimal val)
        {
            if (val < 0)
            {
                return new Tuple<bool, string>(false, "Salary can't be negative");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string> RewriteValidator(char val)
        {
            if (val == char.MaxValue)
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, char> RewriteConverter(string val)
        {
            val = val.Trim();
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, char>(false, "Empty field", char.MinValue);
            }

            if (char.TryParse(val, out char result))
            {
                return new Tuple<bool, string, char>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, char>(false, "The symbol is not of the char type", char.MinValue);
            }
        }
    }
}