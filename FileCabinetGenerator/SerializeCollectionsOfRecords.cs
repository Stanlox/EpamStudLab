using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// class for serialize cillection.
    /// </summary>
    [XmlRoot(IsNullable = false)]
    [XmlInclude(typeof(FileCabinetRecord))]
    [Serializable]
    public class SerializeCollectionsOfRecords
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeCollectionsOfRecords"/> class.
        /// </summary>
        public SerializeCollectionsOfRecords()
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
        public string Records { get; set; } = "Records";

        /// <summary>
        /// Gets collection for serialize.
        /// </summary>
        /// <value>The Records property gets/sets the value.</value>
        [XmlElement]
        public List<FileCabinetRecord> Record { get; }
    }
}
