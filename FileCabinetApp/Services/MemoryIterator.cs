using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.Services
{
    /// <inheritdoc/>
    public class MemoryIterator : IRecordIterator
    {
        private FileCabinetRecord[] records;

        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public MemoryIterator(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.records = records.ToArray();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            return this.records[this.currentIndex++];
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentIndex < this.records.Length;
        }

        /// <inheritdoc/>
        public int GetCount()
        {
            return this.records.Length;
        }
    }
}
