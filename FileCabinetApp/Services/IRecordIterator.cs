using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Records iterator.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets the next element of collection.
        /// </summary>
        /// <returns>The next element of the collection.</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Returns if collection has more elements.
        /// </summary>
        /// <returns>If collection has more elements.</returns>
        bool HasMore();

        /// <summary>
        /// Gets count of element.
        /// </summary>
        /// <returns>Count of element.</returns>
        int GetCount();
    }
}
