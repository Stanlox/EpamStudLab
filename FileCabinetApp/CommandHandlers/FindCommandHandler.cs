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
        private IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        /// <param name="printer">Input printer.</param>
        public FindCommandHandler(IFileCabinetService service, DefaultRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
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
                var parameterValue = parameters.Split(' ').Last().Trim('"');
                var parameterArray = parameters.Split(' ');
                var parameterName = parameterArray[parameterArray.Length - 2];
                switch (parameterName.ToLower(CultureInfo.CurrentCulture))
                {
                    case "firstname":
                        Program.listRecordsInService = this.service.FindByFirstName(parameterValue);
                        this.printer.Print(Program.listRecordsInService);
                        break;
                    case "lastname":
                        Program.listRecordsInService = this.service.FindByLastName(parameterValue);
                        this.printer.Print(Program.listRecordsInService);
                        break;
                    case "dateofbirth":
                        Program.listRecordsInService = this.service.FindByDateOfBirth(parameterValue);
                        this.printer.Print(Program.listRecordsInService);
                        break;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
