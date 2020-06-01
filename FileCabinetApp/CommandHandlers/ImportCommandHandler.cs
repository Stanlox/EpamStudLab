using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler importing records.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
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

            const string name = "import";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                Import(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
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
    }
}
