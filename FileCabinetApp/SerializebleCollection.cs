using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetApp
{
    /// <summary>
    /// class for serialize cillection.
    /// </summary>
    [XmlRoot(IsNullable = false)]
    [XmlInclude(typeof(FileCabinetRecord))]
    [Serializable]
    public class SerializebleCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializebleCollection"/> class.
        /// </summary>
        public SerializebleCollection()
        {
            this.Record = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets or sets an attribute.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        [XmlAttribute]
        public string Records { get; set; }

        /// <summary>
        /// Gets collection for serialize.
        /// </summary>
        /// <value>The Records property gets/sets the value.</value>
        [XmlElement]
        public List<FileCabinetRecord> Record { get; }
    }
}