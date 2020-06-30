using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// contains a method for writing data type of <see cref="FileCabinetRecord"/> in the format csv.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;
        private FileCabinetRecordCsvWriter csvWriter;
        private FileCabinetRecordXmlWriter xmlWriter;
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileCabinetRecordCsvReader fileCabinetRecordCsvReader;
        private FileCabinetRecordXmlReader fileCabinetRecordXmlReader;

        /// <summary>
        /// Gets сollection of records from the program.
        /// </summary>
        /// <value>Сollection of records from the program.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; }

        /// <summary>
        /// Gets list of records from file.
        /// </summary>
        /// <value>list of records from file.</value>
        public IList<FileCabinetRecord> ListFromFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// initializes a record.
        /// </summary>
        /// <param name="fileCabinetRecord">Input record.</param>
        #pragma warning disable SA1201
        public FileCabinetServiceSnapshot(FileCabinetRecord[] fileCabinetRecord)
        {
            this.records = fileCabinetRecord;
            this.Records = new ReadOnlyCollection<FileCabinetRecord>(fileCabinetRecord);
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

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetRecordXmlWriter"/> and list of record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToXml(StreamWriter sw)
        {
            this.xmlWriter = new FileCabinetRecordXmlWriter(XmlWriter.Create(sw));
            foreach (var record in this.records)
            {
                this.list.Add(record);
            }

            this.xmlWriter.Write(this.list);
        }

        /// <summary>
        /// Load data from csv file.
        /// </summary>
        /// <param name="filestream">Input filestream.</param>
        public void LoadFromCsv(FileStream filestream)
        {
            using (StreamReader streamReader = new StreamReader(filestream.Name, Encoding.ASCII))
            {
                this.fileCabinetRecordCsvReader = new FileCabinetRecordCsvReader(streamReader);
                this.ListFromFile = new ReadOnlyCollection<FileCabinetRecord>(this.fileCabinetRecordCsvReader.ReadAll());
            }
        }

        /// <summary>
        /// Load data from xml file.
        /// </summary>
        /// <param name="filestream">Input filestream.</param>
        public void LoadFromXml(FileStream filestream)
        {
            using (XmlReader xmlReader = XmlReader.Create(filestream.Name))
            {
                this.fileCabinetRecordXmlReader = new FileCabinetRecordXmlReader(xmlReader);
                this.ListFromFile = new ReadOnlyCollection<FileCabinetRecord>(this.fileCabinetRecordXmlReader.ReadAll());
            }
        }
    }
}
