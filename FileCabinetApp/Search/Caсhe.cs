using System;
using System.Collections.Generic;
using FileCabinetApp.Services.Comparer;

namespace FileCabinetApp.Search
{
    /// <summary>
    /// Find records in cashe.
    /// </summary>
    public static class Caсhe
    {
        private const string FirstName = "FIRSTNAME";
        private const string LastName = "LASTNAME";
        private const string Gender = "GENDER";
        private const string DateOfBirth = "DATEOFBIRTH";
        private const string Id = "ID";
        private const string Salary = "SALARY";
        private const string Age = "AGE";
        private static readonly Dictionary<decimal, List<FileCabinetRecord>> SalaryDictionary = new Dictionary<decimal, List<FileCabinetRecord>>();
        private static readonly Dictionary<short, List<FileCabinetRecord>> AgeDictionary = new Dictionary<short, List<FileCabinetRecord>>();
        private static readonly Dictionary<char, List<FileCabinetRecord>> GenderDictionary = new Dictionary<char, List<FileCabinetRecord>>(new CharEqualityComparer());
        private static readonly Dictionary<string, List<FileCabinetRecord>> FirstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, List<FileCabinetRecord>> LastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<DateTime, List<FileCabinetRecord>> DateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private static readonly Dictionary<int, List<FileCabinetRecord>> IdrecordDictionary = new Dictionary<int, List<FileCabinetRecord>>();

        private static readonly Tuple<string, Action<string, FileCabinetRecord>>[] Finds = new Tuple<string, Action<string, FileCabinetRecord>>[]
        {
                    new Tuple<string, Action<string, FileCabinetRecord>>(Id, AddInDictionaryId),
                    new Tuple<string, Action<string, FileCabinetRecord>>(FirstName, AddInDictionaryFirstName),
                    new Tuple<string, Action<string, FileCabinetRecord>>(LastName, AddInDictionaryLastName),
                    new Tuple<string, Action<string, FileCabinetRecord>>(DateOfBirth, AddInDictionaryDateOfBirth),
                    new Tuple<string, Action<string, FileCabinetRecord>>(Age, AddInDictionaryAge),
                    new Tuple<string, Action<string, FileCabinetRecord>>(Salary, AddInDictionarySalary),
                    new Tuple<string, Action<string, FileCabinetRecord>>(Gender, AddInDictionaryGender),
        };

        private static IEnumerable<FileCabinetRecord> findRecords;
        private static bool isParse;

        /// <summary>
        /// Find record in cashe by key.
        /// </summary>
        /// <param name="key">Input key.</param>
        /// <param name="value">Input value.</param>
        /// <returns>Found records.</returns>
        public static IEnumerable<FileCabinetRecord> FindRecordInCashe(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            switch (key.ToUpperInvariant())
            {
                case Id:
                    isParse = int.TryParse(value, out int id);
                    IsParse(isParse, id);
                    findRecords = FindById(id);
                    break;
                case FirstName:
                    findRecords = FindByFirstName(value);
                    break;
                case LastName:
                    findRecords = FindByLastName(value);
                    break;
                case DateOfBirth:
                    isParse = DateTime.TryParse(value, out DateTime date);
                    IsParse(isParse, date);
                    findRecords = FindByDateOfBirth(date);
                    break;
                case Gender:
                    isParse = char.TryParse(value, out char gender);
                    IsParse(isParse, gender);
                    findRecords = FindByGender(gender);
                    break;
                case Age:
                    isParse = short.TryParse(value, out short age);
                    IsParse(isParse, age);
                    findRecords = FindByAge(age);
                    break;
                case Salary:
                    isParse = decimal.TryParse(value, out decimal salary);
                    IsParse(isParse, salary);
                    findRecords = FindBySalary(salary);
                    break;
                default:
                    throw new ArgumentException("Unknown field to search");
            }

            return findRecords;
        }

        /// <summary>
        /// Add record in cashe by key.
        /// </summary>
        /// <param name="key">Input key.</param>
        /// <param name="value">Input value.</param>
        /// <param name="record">Input record.</param>
        public static void AddInCashe(object key, string value, FileCabinetRecord record)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var upperCaseKey = key.ToString().ToUpperInvariant();
            var index = Array.FindIndex(Finds, j => j.Item1.Equals(upperCaseKey, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                Finds[index].Item2(value, record);
            }
            else
            {
                throw new ArgumentException("Unknown field to search");
            }
        }

        /// <summary>
        /// Clears cashe.
        /// </summary>
        public static void ClearCashe()
        {
            IdrecordDictionary.Clear();
            FirstNameDictionary.Clear();
            LastNameDictionary.Clear();
            DateofbirthDictionary.Clear();
            SalaryDictionary.Clear();
            AgeDictionary.Clear();
            GenderDictionary.Clear();
        }

        private static bool IsParse(bool isParce, object parameter)
        {
            if (!isParce)
            {
                throw new FormatException($"Failed to convert the {parameter}");
            }
            else
            {
                return true;
            }
        }

        private static IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return FirstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return LastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return DateofbirthDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            return GenderDictionary.TryGetValue(gender, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindById(int id)
        {
            return IdrecordDictionary.TryGetValue(id, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindBySalary(decimal salary)
        {
            return SalaryDictionary.TryGetValue(salary, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static IEnumerable<FileCabinetRecord> FindByAge(short age)
        {
            return AgeDictionary.TryGetValue(age, out List<FileCabinetRecord> result) ? result : new List<FileCabinetRecord>();
        }

        private static void AddInDictionaryFirstName(string key, FileCabinetRecord record)
        {
            if (FirstNameDictionary.ContainsKey(key))
            {
                FirstNameDictionary[key].Add(record);
            }
            else
            {
                FirstNameDictionary.Add(key, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionaryLastName(string key, FileCabinetRecord record)
        {
            if (LastNameDictionary.ContainsKey(key))
            {
                LastNameDictionary[key].Add(record);
            }
            else
            {
                LastNameDictionary.Add(key, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionaryDateOfBirth(string key, FileCabinetRecord record)
        {
            isParse = DateTime.TryParse(key, out DateTime keyDate);
            IsParse(isParse, keyDate);
            if (DateofbirthDictionary.ContainsKey(keyDate))
            {
                DateofbirthDictionary[keyDate].Add(record);
            }
            else
            {
                DateofbirthDictionary.Add(keyDate, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionaryId(string key, FileCabinetRecord record)
        {
            isParse = int.TryParse(key, out int keyId);
            IsParse(isParse, keyId);
            if (!IdrecordDictionary.ContainsKey(keyId))
            {
                IdrecordDictionary.Add(keyId, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionarySalary(string key, FileCabinetRecord record)
        {
            isParse = decimal.TryParse(key, out decimal keySalary);
            IsParse(isParse, keySalary);
            if (SalaryDictionary.ContainsKey(keySalary))
            {
                SalaryDictionary[keySalary].Add(record);
            }
            else
            {
                SalaryDictionary.Add(keySalary, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionaryAge(string key, FileCabinetRecord record)
        {
            isParse = short.TryParse(key, out short keyAge);
            IsParse(isParse, keyAge);
            if (AgeDictionary.ContainsKey(keyAge))
            {
                AgeDictionary[keyAge].Add(record);
            }
            else
            {
                AgeDictionary.Add(keyAge, new List<FileCabinetRecord> { record });
            }
        }

        private static void AddInDictionaryGender(string key, FileCabinetRecord record)
        {
            isParse = char.TryParse(key, out char keyGen);
            IsParse(isParse, keyGen);
            if (GenderDictionary.ContainsKey(keyGen))
            {
               GenderDictionary[keyGen].Add(record);
            }
            else
            {
                GenderDictionary.Add(keyGen, new List<FileCabinetRecord> { record });
            }
        }
    }
}
