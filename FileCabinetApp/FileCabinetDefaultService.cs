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
        /// dependency on IRecordService.
        /// </summary>
        /// <returns>instance DefaultValidator.</returns>
        public static CustomValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
