using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class CommandHandler : CommandHandlerBase
    {
        public override object Handle(AppCommandRequest request)
        {
            return base.Handle(request);
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
                var index = Array.FindIndex(Program.helpMessages, 0, Program.helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(Program.helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in Program.helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            if (Program.fileStream != null)
            {
                Program.fileStream.Close();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s).");
            Console.WriteLine($"{recordsCount.Item2} deleted record(s).");
        }

        private static void Create(string parameters)
        {
            string repeatIfDataIsNotCorrect = parameters;
            try
            {
                UserData();
                Console.WriteLine($"Record # {Program.fileCabinetService.CreateRecord(Program.fileCabinetServiceContext)} is created.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
            {
                Console.WriteLine(ex.Message);
                Program.isCorrect = false;
            }
            finally
            {
                if (!Program.isCorrect)
                {
                    Console.WriteLine("Your data is incorrect, please try again");
                    Program.isCorrect = true;
                    Create(repeatIfDataIsNotCorrect);
                }
            }
        }

        private static void List(string parameters)
        {
            Program.listRecordsInService = Program.fileCabinetService.GetRecords();
            ListRecord(Program.listRecordsInService);
        }

        private static void Edit(string parameters)
        {
            try
            {
                int getNumberEditRecord = int.Parse(parameters, CultureInfo.CurrentCulture);
                Program.listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < Program.listRecordsInService.Count; i++)
                {
                    if (!Program.listRecordsInService.Any(x => x.Id == getNumberEditRecord))
                    {
                        throw new ArgumentException($"#{getNumberEditRecord} record in not found. ");
                    }
                    else if (getNumberEditRecord == Program.listRecordsInService[i].Id)
                    {
                        UserData();
                        Program.fileCabinetService.EditRecord(getNumberEditRecord, Program.fileCabinetServiceContext);
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
                        Program.listRecordsInService = Program.fileCabinetService.FindByFirstName(parameterValue);
                        ListRecord(Program.listRecordsInService);
                        break;
                    case "lastname":
                        Program.listRecordsInService = Program.fileCabinetService.FindByLastName(parameterValue);
                        ListRecord(Program.listRecordsInService);
                        break;
                    case "dateofbirth":
                        Program.listRecordsInService = Program.fileCabinetService.FindByDateOfBirth(parameterValue);
                        ListRecord(Program.listRecordsInService);
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
                Program.snapshot = Program.fileCabinetService.MakeSnapshot();
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
                            Program.snapshot.SaveToCsv(sw);
                            Console.WriteLine($"All records are exported to file {nameFile}");
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var sw = new StreamWriter(nameFile, rewrite))
                        {
                            Program.snapshot.SaveToXml(sw);
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
                    Program.snapshot = Program.fileCabinetService.MakeSnapshot();
                    if (string.Equals(csv, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            Program.snapshot.LoadFromCsv(fileStream);
                            Console.WriteLine($"{Program.snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            Program.fileCabinetService.Restore(Program.snapshot);
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            Program.snapshot.LoadFromXml(fileStream);
                            Console.WriteLine($"{Program.snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            Program.fileCabinetService.Restore(Program.snapshot);
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

                Program.listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < Program.listRecordsInService.Count; i++)
                {
                    if (recordId == Program.listRecordsInService[i].Id)
                    {
                        Program.fileCabinetService.RemoveRecord(recordId);
                        Console.WriteLine($"Record #{recordId} is removed");
                        success = true;
                        break;
                    }
                }

                if (!success)
                {
                    throw new ArgumentException($"Record #{recordId} doesn't exists.");
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
                var tuple = Program.fileCabinetService.PurgeRecord();
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
            Program.fileCabinetServiceContext.FirstName = ReadInput(StringConverter, FirstNameValidator);
            Console.Write("Last Name: ");
            Program.fileCabinetServiceContext.LastName = ReadInput(StringConverter, LastNameValidator);
            Console.Write("Date of birth: ");
            Program.fileCabinetServiceContext.DateOfBirth = ReadInput(DateOfBirthConverter, DateOfBirthValidator);
            Console.Write("Gender (M/W): ");
            Program.fileCabinetServiceContext.Gender = ReadInput(GenderConverter, GenderValidator);
            Console.Write("Age: ");
            Program.fileCabinetServiceContext.Age = ReadInput(AgeConverter, AgeValidator);
            Console.Write("Salary: ");
            Program.fileCabinetServiceContext.Salary = ReadInput(SalaryConverter, SalaryValidator);
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
