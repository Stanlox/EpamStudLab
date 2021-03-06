﻿using System;
using System.IO;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler importing records.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private static FileCabinetServiceSnapshot snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public ImportCommandHandler(IFileCabinetService service)
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

            const string name = "import";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Import(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Import(string parameters)
        {
            const string xml = "xml";
            const string csv = "csv";
            try
            {
                var parameterArray = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parameterArray.Length != 2)
                {
                    throw new ArgumentException("You did not specify the type of file to export or the path to export");
                }

                var fullPath = parameterArray.Last();
                var nameFile = Path.GetFileName(fullPath);
                var extensionFile = Path.GetExtension(nameFile).TrimStart('.');
                var typeFile = parameterArray.First();
                if (!string.Equals(typeFile, xml, StringComparison.OrdinalIgnoreCase) && !string.Equals(typeFile, csv, StringComparison.OrdinalIgnoreCase))
                {
                    bool rezult = false;
                    do
                    {
                        Console.Write("Please, input correct type of file: ");
                        typeFile = Console.ReadLine();
                        rezult = string.Equals(typeFile, xml, StringComparison.OrdinalIgnoreCase) || string.Equals(typeFile, csv, StringComparison.OrdinalIgnoreCase);
                    }
                    while (rezult == false);
                }

                if (typeFile != extensionFile)
                {
                    throw new ArgumentException($"You want to import data from a {nameFile}, but you specified the type {typeFile}");
                }

                if (File.Exists(fullPath))
                {
                    snapshot = this.service.MakeSnapshot();
                    if (string.Equals(csv, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            snapshot.LoadFromCsv(fileStream);
                            Console.WriteLine($"{snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            this.service.Restore(snapshot);
                        }
                    }
                    else if (string.Equals(xml, typeFile, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            snapshot.LoadFromXml(fileStream);
                            Console.WriteLine($"{snapshot.ListFromFile.Count} records were imported from {fullPath}");
                            this.service.Restore(snapshot);
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
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
