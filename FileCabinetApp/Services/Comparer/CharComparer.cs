using System.Collections.Generic;

namespace FileCabinetApp.Services.Comparer
{
    /// <summary>
    /// Implementation of the comparator.
    /// </summary>
    public class CharComparer : IComparer<char>
    {
        /// <summary>
        /// Compares two specified char objects.
        /// </summary>
        /// <param name="x">Input charater 1.</param>
        /// <param name="y">Input character 2.</param>
        /// <returns>Integer that indicates their relative position in the sort order.</returns>
        public int Compare(char x, char y)
        {
            if (char.ToLowerInvariant(x) == char.ToLowerInvariant(y))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
