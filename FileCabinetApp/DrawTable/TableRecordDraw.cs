using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.DrawTable;

namespace FileCabinetApp.TablePrinter
{
    /// <summary>
    /// Contains method for printing the table.
    /// </summary>
    public static class TableRecordDraw
    {
        private const string Delimiter = "|";
        private const string Corner = "+";
        private const char Border = '-';
        private const string IdProperty = "Id";
        private const string FirstNameProperty = "FirstName";
        private const string LastNameProperty = "LastName";
        private const string DateOfBirthProperty = "DateOfBirth";
        private const string AgeProperty = "Age";
        private const string SalaryProperty = "Salary";
        private const string GenderProperty = "Gender";

        private static List<(string property, Func<FileCabinetRecord, string> convert, ContentAlign align)> list =
            new List<(string property, Func<FileCabinetRecord, string> convert, ContentAlign align)>()
        {
                (IdProperty, record => record.Id.ToString(CultureInfo.InvariantCulture), ContentAlign.Right),
                (FirstNameProperty, record => record.FirstName, ContentAlign.Left),
                (LastNameProperty, record => record.LastName, ContentAlign.Left),
                (DateOfBirthProperty, record => record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ContentAlign.Left),
                (AgeProperty, record => record.Age.ToString(CultureInfo.InvariantCulture), ContentAlign.Right),
                (SalaryProperty, record => record.Salary.ToString(CultureInfo.InvariantCulture), ContentAlign.Right),
                (GenderProperty, record => record.Gender.ToString(CultureInfo.InvariantCulture), ContentAlign.Left),
        };

        /// <summary>
        /// Prints the table.
        /// </summary>
        /// <param name="records">Records to print.</param>
        /// <param name="properties">Properties to print.</param>
        public static void PrintTable(IEnumerable<FileCabinetRecord> records, IEnumerable<string> properties)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            var columns = new List<TableColumn>();
            foreach (var property in properties)
            {
                var field = list.Where(x => x.property.Equals(property, StringComparison.InvariantCultureIgnoreCase)).First();
                var column = new TableColumn(
                    field.property,
                    records.Select(record => field.convert(record)),
                    field.align);
                columns.Add(column);
            }

            PrintLineBorder(columns.Select(column => column.MaxLenqth).ToArray());
            var valuesCount = records.Count() + 1;
            for (var i = 0; i < valuesCount; i++)
            {
                var parameter = columns.Select(column => (column.GetNextValue(), column.MaxLenqth, column.ContentAlign)).ToArray();
                PrintLine(parameter);
                PrintLineBorder(columns.Select(column => column.MaxLenqth).ToArray());
            }
        }

        private static void Print(string value, int maxLength, ContentAlign align)
        {
            string resultString;
            switch (align)
            {
                case ContentAlign.Left:
                    resultString = value + new string(' ', maxLength - value.Length);
                    break;
                case ContentAlign.Right:
                    resultString = new string(' ', maxLength - value.Length) + value;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            Console.Write(resultString);
        }

        private static void PrintLine(params (string value, int maxLength, ContentAlign align)[] sourceArray)
        {
            Console.Write(Delimiter);
            foreach (var item in sourceArray)
            {
                Print(item.value, item.maxLength, item.align);
                Console.Write(Delimiter);
            }

            Console.WriteLine();
        }

        private static void PrintLineBorder(int[] length)
        {
            Console.Write(Corner);
            foreach (var item in length)
            {
                Console.Write(new string(Border, item));
                Console.Write(Corner);
            }

            Console.WriteLine();
        }
    }
}