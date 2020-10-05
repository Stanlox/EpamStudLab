using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for export records.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private static FileCabinetServiceSnapshot snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "export";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Export(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
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

        private void Export(string parameters)
        {
            bool rewrite = false;
            var noRewrite = 'n';
            const string xml = "xml";
            const string csv = "csv";
            try
            {
                snapshot = this.service.MakeSnapshot();
                var parameterArray = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parameterArray.Length != 2)
                {
                    throw new ArgumentException("You did not specify the type of file to export or the path to export");
                }

                var fullPath = parameterArray.Last();
                var nameFile = Path.GetFileName(fullPath);
                var typeFile = parameterArray.First();
                var extensionFile = Path.GetExtension(nameFile).TrimStart('.');
                if (typeFile != extensionFile)
                {
                    throw new ArgumentException($"You want to export data to a {nameFile}, but you specified the type {typeFile}");
                }

                if (File.Exists(fullPath))
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
                        using (var sw = new StreamWriter(fullPath, rewrite))
                        {
                            snapshot.SaveToCsv(sw);
                            Console.WriteLine($"All records are exported to file {nameFile}");
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var sw = new StreamWriter(fullPath, rewrite))
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
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
