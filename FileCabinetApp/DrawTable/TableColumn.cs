using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.DrawTable
{
    /// <summary>
    /// Represents column of the table.
    /// </summary>
    public class TableColumn
    {
        private int currentIndex = 0;

        private string[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumn"/> class.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="values">Values.</param>
        /// <param name="align">Content align.</param>
        public TableColumn(string columnName, IEnumerable<string> values, ContentAlign align)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var tmpList = new List<string>();
            tmpList.Add(columnName);
            tmpList.AddRange(values);
            this.values = tmpList.ToArray();
            this.MaxLenqth = this.values.Max(str => str.Length);
            this.ContentAlign = align;
        }

        /// <summary>
        /// Gets or sets max length of values.
        /// </summary>
        /// <value> Max length of values.</value>
        public int MaxLenqth { get;  set; }

        /// <summary>
        /// Gets or sets contant align.
        /// </summary>
        /// <value> Contant align.</value>
        public ContentAlign ContentAlign { get; set; }

        /// <summary>
        /// Gets the next value.
        /// </summary>
        /// <returns>Value.</returns>
        public string GetNextValue()
        {
            if (this.currentIndex == this.values.Length)
            {
                throw new InvalidOperationException();
            }

            return this.values[this.currentIndex++];
        }
    }
}
