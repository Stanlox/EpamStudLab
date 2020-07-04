using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Search;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for delete a record.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Separator = "where";
        private const string ParametersTemplate = @"(\bwhere\b)\s([a-zA-Z0-9_]{1,10}\s?\=\s?\'[a-zA-Z0-9_]{1,10}\'\,?\s?(\bor\b)?(\band\b)?\s?){1,10}";
        private const string Or = "or";
        private const string And = "and";
        private const string CommandName = "delete";
        private readonly StringBuilder builder = new StringBuilder();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FindRecordInService find;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.find = new FindRecordInService(service, CommandName, ref this.list);
        }

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

            const string name = "delete";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                if (this.service is FileCabinetMemoryService)
                {
                    Caсhe.ClearCashe();
                }

                this.Delete(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static bool IsRightCommandSyntax(string template, string parameters)
        {
            return Regex.IsMatch(parameters, template);
        }

        private static IEnumerable<(string key, string value)> GetKeyValuePairs(string source)
        {
            const int keyIndex = 0;
            const int valueIndex = 1;
            var result = new List<(string key, string value)>();
            var keyValuePairs = source.Split(",");
            foreach (var keyValuePair in keyValuePairs)
            {
                var keyValueArray = keyValuePair.Split('=');
                result.Add((keyValueArray[keyIndex].Trim(), keyValueArray[valueIndex].Trim('\'', ' ')));
            }

            return result;
        }

        private void Delete(string parameters)
        {
            try
            {
                if (!IsRightCommandSyntax(ParametersTemplate, parameters))
                {
                    throw new FormatException("Invalid command format of 'delete' command.");
                }

                var parametersArray = parameters.Trim().Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                this.FindRecord(string.Join(string.Empty, parametersArray));

                if (this.list.Count == 0)
                {
                    throw new ArgumentException("Record(s) with these parameters was found");
                }

                this.Print();
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Invalid command format of 'delete' command.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Print()
        {
            var sortedRecords = this.list.OrderBy(record => record.Id).ToList();
            for (int j = 0; j < sortedRecords.Count; j++)
            {
                this.service.RemoveRecord(sortedRecords[j].Id);
                if (j + 1 == sortedRecords.Count)
                {
                    this.builder.Append($"# {sortedRecords[j].Id} ");
                }
                else
                {
                    this.builder.Append($"# {sortedRecords[j].Id}, ");
                }
            }

            if (sortedRecords.Count == 1)
            {
                Console.WriteLine($"Record {this.builder.ToString()}is deleted.");
            }
            else
            {
                Console.WriteLine($"Record {this.builder.ToString()}are deleted.");
            }
        }

        private void FindRecord(string request)
        {
            if (request.Contains(Or, StringComparison.InvariantCultureIgnoreCase) || request.Contains(And, StringComparison.InvariantCultureIgnoreCase))
            {
                var isContainsAnd = this.find.ContainsAnd(request, And);
                var parameters = request.Trim().Split(' ');

                var separators = new string[] { Or, And };
                var parametersArray = parameters
                    .Select(item => separators
                    .Contains(item) ? item.Replace(item, ",", StringComparison.InvariantCultureIgnoreCase) : item).ToArray();

                var countSeparators = 0;
                for (int i = 0; i < parametersArray.Length; i++)
                {
                    if (parametersArray[i] != parameters[i])
                    {
                        countSeparators++;
                    }
                }

                if (countSeparators > 1)
                {
                    throw new FormatException("Invalid command format of 'delete' command.");
                }

                var valueAndProperty = string.Join(string.Empty, parametersArray);
                var keyValuePairs = GetKeyValuePairs(valueAndProperty);
                foreach (var pair in keyValuePairs)
                {
                    this.find.FindRecordByKey(pair.key, pair.value);
                }

                if (isContainsAnd)
                {
                    foreach (var pair in keyValuePairs)
                    {
                        this.find.FindRecordByMatchingCriteria(pair.key, pair.value);
                    }
                }
            }
            else
            {
                var keyValuePairs = GetKeyValuePairs(request.Trim());
                foreach (var pair in keyValuePairs)
                {
                    this.find.FindRecordByKey(pair.key, pair.value);
                }
            }
        }
    }
}
