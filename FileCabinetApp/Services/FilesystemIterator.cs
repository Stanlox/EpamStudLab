using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.Services
{
    /// <inheritdoc/>
    public class FilesystemIterator : IRecordIterator
    {
        private readonly FileCabinetFilesystemService service;
        private int currentIndex;
        private int[] positions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="positions">Collection with positions.</param>
        public FilesystemIterator(FileCabinetFilesystemService service, IEnumerable<int> positions)
        {
            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.positions = positions.ToArray();
        }

        /// <inheritdoc/>
        public int GetCount()
        {
            return this.positions.Length;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
           var record = this.service.ReadByPosition(this.positions[this.currentIndex++]);
           return record;
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentIndex < this.positions.Length;
        }
    }
}
