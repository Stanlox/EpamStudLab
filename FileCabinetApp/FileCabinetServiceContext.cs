using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// this class is intended to be passed as an object.
    /// </summary>
    public class FileCabinetServiceContext
    {
        /// <summary>
        /// Gets or sets properties FirstName.
        /// </summary>
        /// <value>The FirstName property gets/sets the value.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets properties LastName.
        /// </summary>
        /// <value>The LastName property gets/sets the value.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets properties DateOfBirth.
        /// </summary>
        /// <value>The DateOfBirth property gets/sets the value.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets properties Gender.
        /// </summary>
        /// <value>The Gender property gets/sets the value.</value>
        public char Gender { get; set; }

        /// <summary>
        /// Gets or sets properties Age.
        /// </summary>
        /// <value>The Age property gets/sets the value.</value>
        public short Age { get; set; }

        /// <summary>
        /// Gets or sets properties Salary.
        /// </summary>
        /// <value>The Salary property gets/sets the value.</value>
        public decimal Salary { get; set; }
    }
}
