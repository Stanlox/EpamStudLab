﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler command help.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records.", "The 'stat' command prints statistics." },
            new string[] { "create", "creates new user.", "The 'create' command creates the record." },
            new string[] { "list", "prints a list of records added to the service.", "The 'list' command prints all records." },
            new string[]
            {
                "find", "finds record by the specified value.", "The 'find' command finds all records by specified value." +
                "\nFull command: find<property name> <value>" +
                "\nAcceptable properties: firstName, lastName, dateOfBirth. Case doesn't matter." +
                "\nValue is the string with the value." +
                "\nExample: find firstName \"Maxim\"",
            },
            new string[]
            {
                "export", "exports all records to a file.", "The 'export' command exports all records to a file." +
                "\nFull command: export csv/xml<path>" +
                "\nThe path starts with the file name and ends with its extension. " +
                "\nFile format must be the same with first parameter." +
                "\nExample: export csv data.csv",
            },
            new string[]
            {
                "import", "imports all records from a file.", "The 'import' command imports all records from a file." +
                "\nFull command: import csv/xml <path>" +
                "\nThe path starts with the file name and ends with its extension." +
                "\nFile format must be the same with the first parameter." +
                "\nExample: import xml records.xml",
            },
            new string[] { "purge", "deleting \"voids\" in the data file.", "The 'purge' command clears records in a file marked with bit 0." },
            new string[]
            {
                "insert", "inserts a record with the specified parameters.", "The 'insert' command creates the record." +
                "\nFull command: (all fields must be listed in any order) values (the value corresponding to the fields)." +
                "\nExamples: insert (firstName,dateofbirth,lastName,salary,age,gender) values ('maxim','12/12/2012','bandaruk','1000','19','M')",
            },
            new string[]
            {
                "delete", "deletes records by conditions. ", "The 'delete' command deletes records by conditions." +
                "\nFull command: delete where <field> or/and/without logical operators = 'value'." +
                "\nExamples: delete where dateofbirth = '11/11/2000'" +
                "\n delete where firstname = 'maxim' and lastname = 'bandaruk'" +
                "\n delete where id='1' or salary = '1000'",
            },
            new string[]
            {
                "update", "updates records.", "The 'update' command updates records by conditions." +
                "\nFull command: update set <field(s)> = value(s) where <field(s)> = value(s) " +
                "\nExample: update set firstname = 'ivan' where lastname = 'ivanov' and age = '15'" +
                "\nupdate set salary = '2000', firstname = 'Vitya' where id = '1'",
            },
        };

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

            const string name = "help";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                PrintHelp(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
