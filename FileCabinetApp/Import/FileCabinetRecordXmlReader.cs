﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains methods for reading records from a xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private XmlReader xmlReader;
        private XmlSerializer serializer = new XmlSerializer(typeof(SerializebleCollection));

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="xmlReader">Xml reader.</param>
        public FileCabinetRecordXmlReader(XmlReader xmlReader)
        {
            this.xmlReader = xmlReader ?? throw new ArgumentNullException(nameof(xmlReader));
        }

        /// <summary>
        /// Read all data from xml file.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            SerializebleCollection serializebleCollection = new SerializebleCollection();
            IList<FileCabinetRecord> list = new List<FileCabinetRecord>();
            serializebleCollection = (SerializebleCollection)this.serializer.Deserialize(this.xmlReader);

            for (int i = 0; i < serializebleCollection.Record.Count; i++)
            {
                list.Add(serializebleCollection.Record[i]);
            }

            return list;
        }
    }
}
