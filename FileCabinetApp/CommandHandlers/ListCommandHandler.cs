using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler command list.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        /// <param name="printer">Input printer.</param>
        public ListCommandHandler(IFileCabinetService service, DefaultRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "list";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.printer.Print(Program.listRecordsInService);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        //private void List(string parameters)
        //{
        //    Program.listRecordsInService = this.service.GetRecords();
        //    this.printer.Print(Program.listRecordsInService);
        //}
    }
}
