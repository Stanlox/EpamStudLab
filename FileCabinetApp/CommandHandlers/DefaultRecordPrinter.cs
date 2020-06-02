using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                var builder = new StringBuilder();
                builder.Append($"{item.Id}, ");
                builder.Append($"{item.FirstName}, ");
                builder.Append($"{item.LastName}, ");
                builder.Append($"{item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{item.Gender}, ");
                builder.Append($"{item.Age}, ");
                builder.Append($"{item.Salary}");
                Console.WriteLine("#" + builder.ToString());
            }
        }
    }
}
