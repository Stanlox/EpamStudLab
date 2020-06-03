﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultDateOfBirthValidator
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);

        /// <summary>
        /// checks the date of birth range.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <exception cref="ArgumentException">thrown when <paramref name="parameters.DateOfBirth"/> is the wrong range.</exception>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            if (DateTime.Compare(DateTime.Now, parameters.DateOfBirth) < 0 || DateTime.Compare(MinDate, parameters.DateOfBirth) > 0)
            {
                throw new ArgumentException("Invalid Date", $"{nameof(parameters.DateOfBirth)}");
            }
        }
    }
}
