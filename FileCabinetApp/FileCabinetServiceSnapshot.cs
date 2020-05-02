using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace FileCabinetApp
{
    /// <summary>
    /// contains a method for writing data type of <see cref="FileCabinetRecord"/> in the format csv.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;
        private FileCabinetRecordCsvWriter csvWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// initializes a record.
        /// </summary>
        /// <param name="fileCabinetRecord">Input record.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] fileCabinetRecord)
        {
            this.records = fileCabinetRecord;
        }

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetRecordCsvWriter"/> and record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToCsv(StreamWriter sw)
        {
            this.csvWriter = new FileCabinetRecordCsvWriter(sw);
            foreach (var record in this.records)
            {
                this.csvWriter.Write(record);
            }
        }
    }
}
