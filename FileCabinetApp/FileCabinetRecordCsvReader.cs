using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> list = new List<FileCabinetRecord>();
            string line;
            string[] values;
            this.reader.BaseStream.Position = 0;
            while (!this.reader.EndOfStream)
            {
                FileCabinetRecord record = new FileCabinetRecord();
                line = this.reader.ReadLine();
                values = line.Split(",");
                record.Id = int.Parse(values[0], CultureInfo.InvariantCulture);
                record.FirstName = values[1];
                record.LastName = values[2];
                record.DateOfBirth = DateTime.Parse(values[3], CultureInfo.InvariantCulture);
                record.Gender = char.Parse(values[4]);
                record.Age = short.Parse(values[5], CultureInfo.InvariantCulture);
                record.Salary = decimal.Parse(values[6], CultureInfo.InvariantCulture);
                list.Add(record);
            }

            return list;
        }
    }
}
