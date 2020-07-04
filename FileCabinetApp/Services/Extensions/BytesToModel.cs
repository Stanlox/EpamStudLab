using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.Services.Extensions
{
    /// <summary>
    /// Сontains methods for converting bytes to records.
    /// </summary>
    public static class BytesToModel
    {
        private static short status;
        private static int maxStringLength;
        private static List<FileCabinetRecord> isDeletedlist;

        /// <summary>
        /// Set fields.
        /// </summary>
        /// <param name="deletedStatus">Status of deleting a record.</param>
        /// <param name="maxstringlength">The maximal length of the string.</param>
        /// <param name="deletedlist">List of deleted records.</param>
        public static void SetFields(ref short deletedStatus, int maxstringlength, ref List<FileCabinetRecord> deletedlist)
        {
            status = deletedStatus;
            maxStringLength = maxstringlength;
            isDeletedlist = deletedlist;
        }

        /// <summary>
        /// Сonverts an array of bytes to a single record.
        /// </summary>
        /// <param name="bytes">Input bytes.</param>
        /// <returns>Record.</returns>
        public static FileCabinetRecord ToFileCabinetRecord(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    binaryReader.BaseStream.Position = 0;
                    FileCabinetRecord record = new FileCabinetRecord();
                    status = binaryReader.ReadInt16();
                    record.Id = binaryReader.ReadInt32();
                    var firstNameBuffer = binaryReader.ReadBytes(maxStringLength);
                    var lastNameBuffer = binaryReader.ReadBytes(maxStringLength);
                    var firstNameLength = Encoding.ASCII.GetString(firstNameBuffer, 0, firstNameBuffer.Length);
                    var lastNameLength = Encoding.ASCII.GetString(lastNameBuffer, 0, lastNameBuffer.Length);
                    record.FirstName = firstNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                    record.LastName = lastNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                    var yearOfBirth = binaryReader.ReadInt32();
                    var monthOfBirth = binaryReader.ReadInt32();
                    var dayOfBirth = binaryReader.ReadInt32();
                    record.DateOfBirth = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);
                    record.Age = binaryReader.ReadInt16();
                    record.Salary = binaryReader.ReadDecimal();
                    record.Gender = binaryReader.ReadChar();
                    return record;
                }
            }
        }

        /// <summary>
        /// Converts an array of bytes to list a record.
        /// </summary>
        /// <param name="bytes">Input array of bytes.</param>
        /// <returns>List of records.</returns>
        public static List<FileCabinetRecord> ToListFileCabinetRecord(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            List<FileCabinetRecord> recordList = new List<FileCabinetRecord>();

            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    binaryReader.BaseStream.Position = 0;
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        FileCabinetRecord record = new FileCabinetRecord();
                        status = binaryReader.ReadInt16();
                        record.Id = binaryReader.ReadInt32();
                        var firstNameBuffer = binaryReader.ReadBytes(maxStringLength);
                        var lastNameBuffer = binaryReader.ReadBytes(maxStringLength);
                        var firstNameLength = Encoding.ASCII.GetString(firstNameBuffer, 0, firstNameBuffer.Length);
                        var lastNameLength = Encoding.ASCII.GetString(lastNameBuffer, 0, lastNameBuffer.Length);
                        record.FirstName = firstNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                        record.LastName = lastNameLength.Replace("\0", string.Empty, StringComparison.OrdinalIgnoreCase);
                        var yearOfBirth = binaryReader.ReadInt32();
                        var monthOfBirth = binaryReader.ReadInt32();
                        var dayOfBirth = binaryReader.ReadInt32();
                        record.DateOfBirth = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);
                        record.Age = binaryReader.ReadInt16();
                        record.Salary = binaryReader.ReadDecimal();
                        record.Gender = binaryReader.ReadChar();
                        if (status == 1)
                        {
                            var findRecord = isDeletedlist.Find(item => item.Id == record.Id);
                            if (!isDeletedlist.Contains(findRecord))
                            {
                                isDeletedlist.Add(record);
                            }
                        }
                        else
                        {
                            recordList.Add(record);
                        }
                    }
                }
            }

            return recordList;
        }
    }
}
