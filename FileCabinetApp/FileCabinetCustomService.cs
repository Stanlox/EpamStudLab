using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// inherits the class implementation FileCabinetService.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        /// <returns>instance CustomValidator.</returns>
        public FileCabinetCustomService()
            : base(new DefaultValidator())
        {
        }
    }
}
