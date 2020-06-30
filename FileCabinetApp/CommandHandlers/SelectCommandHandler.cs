using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Search;
using FileCabinetApp.TablePrinter;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler for shows records.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Where = "where";
        private const string And = "and";
        private const string Or = "or";
        private const int BeforeWhere = 0;
        private const int AfterWhere = 1;
        private const string ParametersTemplateWithCondition = @"([a-zA-Z0-9_]{1,60}\,?\s){1,7}(\bwhere\b)\s([a-zA-Z0-9_]{1,60}\s?\=\s?\'[a-zA-Z0-9_/]{1,60}\'\,?)\s(and|or)\s([a-zA-Z0-9_]{1,60}\s?\=\s?\'[a-zA-Z0-9_/]{1,60}\'\,?)";
        private const string ParametersTemplateWithoutCondition = @"([a-zA-Z0-9_]{1,60}\,?\s?){1,7}";
        private const string InvalidFormat = "Invalid command format of 'select' command.";
        private const string CommandName = "select";
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private IEnumerable<FileCabinetRecord> recordsToShow;
        private IEnumerable<string> propertiesToShow;
        private FindRecordInService find;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.find = new FindRecordInService(service, CommandName, ref this.list);
        }

        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input record.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "select";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Select(request.Parameters);
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

        private void Select(string parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(parameters);
                }

                var parametersArray = parameters.Split(Where, StringSplitOptions.None);

                if (parametersArray.Length == 1)
                {
                    if (!IsRightCommandSyntax(ParametersTemplateWithoutCondition, string.Join(string.Empty, parametersArray)))
                    {
                        throw new FormatException(InvalidFormat);
                    }

                    this.recordsToShow = this.service.GetRecords();
                }
                else if (!IsRightCommandSyntax(ParametersTemplateWithCondition, parameters))
                {
                    throw new FormatException(InvalidFormat);
                }

                this.propertiesToShow = parametersArray[BeforeWhere].Trim().Split(",").Select(prop => prop.Trim().ToUpperInvariant()).AsEnumerable();

                if (parametersArray.Length == 2)
                {
                    var isContainsAnd = this.find.ContainsAnd(parametersArray[AfterWhere], And);
                    var separators = new string[] { Or, And };
                    var parametersAfterWhere = parametersArray[AfterWhere].Trim().Split(' ');
                    var parametersAfterWhereWithoutSeparators = parametersAfterWhere.Select(item => separators
                    .Contains(item) ? item.Replace(item, ",", StringComparison.InvariantCultureIgnoreCase) : item).ToArray();

                    var keyValuePairs = GetKeyValuePairs(string.Join(string.Empty, parametersAfterWhereWithoutSeparators)).ToList();

                    foreach (var pair in keyValuePairs)
                    {
                        if (this.service is FileCabinetMemoryService)
                        {
                            var recordsFromCashe = Caсhe.FindRecordInCashe(pair.key, pair.value);
                            if (!recordsFromCashe.Any())
                            {
                                var recordsFromSrvice = this.find.FindRecordByKey(pair.key, pair.value);
                                foreach (var record in recordsFromSrvice)
                                {
                                    Caсhe.AddInCashe(pair.key, pair.value, record);
                                }
                            }
                            else
                            {
                                foreach (var record in recordsFromCashe)
                                {
                                    if (!this.list.Exists(x => x.Id == record.Id))
                                    {
                                        this.list.Add(record);
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.find.FindRecordByKey(pair.key, pair.value);
                        }
                    }

                    if (isContainsAnd)
                    {
                        foreach (var pair in keyValuePairs)
                        {
                            this.find.FindRecordByMatchingCriteria(pair.key, pair.value);
                        }
                    }

                    this.recordsToShow = this.list.OrderBy(record => record.Id).AsEnumerable();
                }

                TableRecordDraw.PrintTable(this.recordsToShow, this.propertiesToShow);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
