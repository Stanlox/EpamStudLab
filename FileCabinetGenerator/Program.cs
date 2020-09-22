using System;
using System.Data;
using System.IO;
using System.Linq;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Сlass designed for processing input parameters and generating data.
    /// </summary>
    public static class Program
    {
        private static bool isRunning = true;
        private static string outputFileName;
        private static string outputFormatType;
        private static int valueToStart;
        private static bool isCorrectData = true;
        private static ServiceGenerator serviceGenerator = new ServiceGenerator();
        private static FileCabinetServiceGeneratorSnapshot fileCabinetServiceGeneratorSnapshot;
        private static int amountOfGeneratedRecords;

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0)
                {
                    var parameterCommandLine = string.Join(' ', args);
                    var parameterName = parameterCommandLine.Trim(' ').Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                    var pairs = parameterName.Select((value, key) => new { Index = key, Value = value })
                        .Where(i => i.Index % 2 == 0)
                        .Select(pair => new { Key = pair.Value, Value = parameterName[pair.Index + 1] });

                    foreach (var pair in pairs)
                    {
                        switch (pair.Key)
                        {
                            case "--output-type":
                            case "-t":
                                outputFormatType = pair.Value;
                                break;
                            case "--output":
                            case "-o":
                                outputFileName = pair.Value;
                                break;
                            case "-a":
                            case "--records-amount":
                                isCorrectData = int.TryParse(pair.Value, out amountOfGeneratedRecords);
                                break;
                            case "-i":
                            case "--start-id":
                                isCorrectData = int.TryParse(pair.Value, out valueToStart);
                                break;
                            default:
                                throw new ArgumentException("You entered an unknown command or incorrect format of the entered data");
                        }
                    }

                    if (!isCorrectData)
                    {
                        Console.WriteLine("Сheck the entered values");
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Сheck the entered values");
                Environment.Exit(0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }

            CreateRecord();
            Export();
            if (!isRunning)
            {
                Console.WriteLine("Check the file path you entered");
            }
            else
            {
                Console.WriteLine($"{amountOfGeneratedRecords} records were written to {outputFileName}");
            }
        }

        private static void CreateRecord()
        {
            serviceGenerator.CreateRecordRandomValues(amountOfGeneratedRecords, valueToStart);
        }

        private static void Export()
        {
            const string csv = "csv";
            const string xml = "xml";
            string shortPath = Path.GetFileName(outputFileName);
            string extensionFile = Path.GetExtension(shortPath).TrimStart('.');
            try
            {
                fileCabinetServiceGeneratorSnapshot = serviceGenerator.MakeSnapshot();
                try
                {
                    if (string.Equals(csv, outputFormatType, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(csv, extensionFile, StringComparison.OrdinalIgnoreCase))
                        {
                            using (var sw = new StreamWriter(outputFileName))
                            {
                                fileCabinetServiceGeneratorSnapshot.SaveToCsv(sw);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The file extension does not match the selected output format type");
                        }
                    }
                    else if (string.Equals(xml, outputFormatType, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(xml, extensionFile, StringComparison.OrdinalIgnoreCase))
                        {
                            using (var sw = new StreamWriter(outputFileName))
                            {
                                fileCabinetServiceGeneratorSnapshot.SaveToXml(sw);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The file extension does not match the selected output format type");
                        }
                    }
                    else
                    {
                        isRunning = false;
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Export failed: can't open file {outputFileName}");
                    isRunning = false;
                }
                catch (ArgumentException ex)
                {
                    isRunning = false;
                    Console.WriteLine(ex.Message);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Enter the file extension and his name or path");
                isRunning = false;
            }
        }
    }
}