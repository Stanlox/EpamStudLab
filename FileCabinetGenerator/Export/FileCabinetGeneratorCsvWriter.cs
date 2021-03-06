﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// contains methods for writing records to a file.
    /// </summary>
    public class FileCabinetGeneratorCsvWriter
    {
        private TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetGeneratorCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Thread.</param>
        public FileCabinetGeneratorCsvWriter(TextWriter writer)
        {
            this.textWriter = writer;
        }

        /// <summary>
        /// writes to a record file.
        /// </summary>
        /// <param name="fileCabinetRecord">the record of a <see cref="FileCabinetRecord"/> type.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var builder = new StringBuilder();
            builder.Append($"{fileCabinetRecord.Id},");
            builder.Append($"{fileCabinetRecord.FirstName},");
            builder.Append($"{fileCabinetRecord.LastName},");
            builder.Append($"{fileCabinetRecord.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},");
            builder.Append($"{fileCabinetRecord.Gender},");
            builder.Append($"{fileCabinetRecord.Age},");
            builder.Append($"{fileCabinetRecord.Salary}");
            this.textWriter.WriteLine(builder.ToString());
        }
    }
}
