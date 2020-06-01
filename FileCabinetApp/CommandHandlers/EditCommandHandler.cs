using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for edit record.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "edit";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                Edit(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Edit(string parameters)
        {
            try
            {
                int getNumberEditRecord = int.Parse(parameters, CultureInfo.CurrentCulture);
                Program.listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < Program.listRecordsInService.Count; i++)
                {
                    if (!Program.listRecordsInService.Any(x => x.Id == getNumberEditRecord))
                    {
                        throw new ArgumentException($"#{getNumberEditRecord} record in not found. ");
                    }
                    else if (getNumberEditRecord == Program.listRecordsInService[i].Id)
                    {
                        UserData();
                        Program.fileCabinetService.EditRecord(getNumberEditRecord, Program.fileCabinetServiceContext);
                        Console.WriteLine($"Record #{getNumberEditRecord} is updated.");
                        break;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UserData()
        {
            Console.Write("First name: ");
            Program.fileCabinetServiceContext.FirstName = ReadInput(StringConverter, FirstNameValidator);
            Console.Write("Last Name: ");
            Program.fileCabinetServiceContext.LastName = ReadInput(StringConverter, LastNameValidator);
            Console.Write("Date of birth: ");
            Program.fileCabinetServiceContext.DateOfBirth = ReadInput(DateOfBirthConverter, DateOfBirthValidator);
            Console.Write("Gender (M/W): ");
            Program.fileCabinetServiceContext.Gender = ReadInput(GenderConverter, GenderValidator);
            Console.Write("Age: ");
            Program.fileCabinetServiceContext.Age = ReadInput(AgeConverter, AgeValidator);
            Console.Write("Salary: ");
            Program.fileCabinetServiceContext.Salary = ReadInput(SalaryConverter, SalaryValidator);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string> FirstNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input FirstName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string, string> StringConverter(string val)
        {
            return new Tuple<bool, string, string>(true, val, val);
        }

        private static Tuple<bool, string> LastNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input LastName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime val)
        {
            if (val == DateTime.MinValue)
            {
                return new Tuple<bool, string>(false, "Сheck input DateOfBirth");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, DateTime> DateOfBirthConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, DateTime>(false, "Empty field", DateTime.MinValue);
            }

            if (DateTime.TryParse(val, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(false, val, result);
            }
        }

        private static Tuple<bool, string> GenderValidator(char val)
        {
            if (val == char.MaxValue)
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, char> GenderConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, char>(false, "Empty field", char.MinValue);
            }

            if (char.TryParse(val, out char result))
            {
                return new Tuple<bool, string, char>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, char>(false, "The symbol is not of the char type", char.MinValue);
            }
        }

        private static Tuple<bool, string> AgeValidator(short val)
        {
            if (val <= 0)
            {
                return new Tuple<bool, string>(false, "Age can't be negative or equal 0");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, short> AgeConverter(string val)
        {
            if (short.TryParse(val, out short result))
            {
                return new Tuple<bool, string, short>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, short>(false, val, 0);
            }
        }

        private static Tuple<bool, string, decimal> SalaryConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, decimal>(false, "Empty string", 0);
            }
            else if (decimal.TryParse(val, out decimal result))
            {
                return new Tuple<bool, string, decimal>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, decimal>(false, val, 0);
            }
        }

        private static Tuple<bool, string> SalaryValidator(decimal val)
        {
            if (val < 0)
            {
                return new Tuple<bool, string>(false, "Salary can't be negative");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }
    }
}
