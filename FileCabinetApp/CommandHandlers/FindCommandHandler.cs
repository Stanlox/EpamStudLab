using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for find a record.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> print;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        /// <param name="print">Print method.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> print)
            : base(service)
        {
            this.print = print ?? throw new ArgumentNullException(nameof(print));
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

            const string name = "find";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Find(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Find(string parameters)
        {
            try
            {
                bool isConverted;
                var parameterValue = parameters.Split(' ').Last().Trim('"');
                var parameterArray = parameters.Split(' ');
                var parameterName = parameterArray[parameterArray.Length - 2];
                IEnumerable<FileCabinetRecord> findRecord;
                switch (parameterName.ToLower(CultureInfo.CurrentCulture))
                {
                    case "firstname":
                        findRecord = this.service.FindByFirstName(parameterValue);
                        this.print(findRecord);
                        break;
                    case "lastname":
                        findRecord = this.service.FindByLastName(parameterValue);
                        this.print(findRecord);
                        break;
                    case "dateofbirth":
                        isConverted = DateTime.TryParse(parameterValue, out DateTime date);

                        if (!isConverted)
                        {
                            throw new FormatException("Incorrect syntax Date of birth");
                        }

                        findRecord = this.service.FindByDateOfBirth(date);
                        this.print(findRecord);
                        break;
                    default:
                        Console.WriteLine("Wrong search field");
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Please, input enter the search field and value");
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
    }
}
