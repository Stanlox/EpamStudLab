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
        /// dependency on IRecordService.
        /// </summary>
        /// <returns>instance CustomValidator.</returns>
        public static CustomValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
