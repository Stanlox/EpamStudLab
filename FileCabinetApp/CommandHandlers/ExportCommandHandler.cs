﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for export records.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
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
                Export(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
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
    }
}
