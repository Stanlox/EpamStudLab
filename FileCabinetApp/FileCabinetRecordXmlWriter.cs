using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// serialize with help xmlWriter.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private XmlSerializer serializer = new XmlSerializer(typeof(SerializebleCollection));
        private XmlWriter xmlWriter;
        private SerializebleCollection serializebleCollection = new SerializebleCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">XmlWriter.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.xmlWriter = writer;
        }

        /// <summary>
        /// Serialise records to a file.
        /// </summary>
        /// <param name="records">list <see cref="FileCabinetRecord"/>.</param>
        public void Write(List<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                this.serializebleCollection.Records.Add(item);
            }

            this.serializer.Serialize(this.xmlWriter, this.serializebleCollection);
        }
    }
}
