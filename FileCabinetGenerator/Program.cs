using System;
using System.Linq;

namespace FileCabinetGenerator
{
    public static class Program
    {
        private static string outputFileName;
        private static string outputFormatType;
        private static int amountOfGeneratedRecords;
        private static int valueToStart;
        private static bool isCorrectData = true;

        private static void Main(string[] args)
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
                            Console.WriteLine("You entered an unknown command");
                            break;
                    }
                }

                if (!isCorrectData)
                {
                    Console.WriteLine("Сheck the entered values");
                }
            }

            Console.WriteLine($"{amountOfGeneratedRecords} records were written to {outputFileName}");
            Console.ReadKey();
        }
    }
}