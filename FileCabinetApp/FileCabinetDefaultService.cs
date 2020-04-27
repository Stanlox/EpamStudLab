using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// inherits the class implementation FileCabinetService.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        /// <returns>instance DefaultValidator.</returns>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}
