using System.Collections.Generic;

namespace FileCabinetApp.Services.Comparer
{
    /// <summary>
    /// Implementation of the comparator.
    /// </summary>
    public class CharEqualityComparer : IEqualityComparer<char>
    {
        /// <inheritdoc/>
        public bool Equals(char c1, char c2)
        {
            return char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);
        }

        /// <inheritdoc/>
        public int GetHashCode(char c1)
        {
            return char.ToLowerInvariant(c1).GetHashCode();
        }
    }
}
