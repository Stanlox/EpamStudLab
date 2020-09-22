using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// contains a method for writing data type of <see cref="FileCabinetRecord"/> in the format csv.
    /// </summary>
    public class FileCabinetServiceGeneratorSnapshot
    {
        private FileCabinetRecord[] records;
        private FileCabinetGeneratorCsvWriter csvWriter;
        private FileCabinetGeneratorXmlWriter xmlWriter;
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private XmlWriterSettings settings = new XmlWriterSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceGeneratorSnapshot"/> class.
        /// initializes a record.
        /// </summary>
        /// <param name="fileCabinetRecord">Input record.</param>
        public FileCabinetServiceGeneratorSnapshot(FileCabinetRecord[] fileCabinetRecord)
        {
            this.records = fileCabinetRecord;
            this.settings.OmitXmlDeclaration = true;
            this.settings.Indent = true;
            this.settings.NewLineOnAttributes = true;
        }

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetGeneratorCsvWriter"/> and record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToCsv(StreamWriter sw)
        {
            this.csvWriter = new FileCabinetGeneratorCsvWriter(sw);
            foreach (var record in this.records)
            {
                this.csvWriter.Write(record);
            }
        }

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetGeneratorXmlWriter"/> and list of record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToXml(StreamWriter sw)
        {
            this.xmlWriter = new FileCabinetGeneratorXmlWriter(XmlWriter.Create(sw, this.settings));
            foreach (var record in this.records)
            {
                this.list.Add(record);
            }

            this.xmlWriter.Write(this.list);
        }
    }
}
