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
        private static ReadOnlyCollection<FileCabinetRecord> listRecordsInService;
        private Action<IEnumerable<FileCabinetRecord>> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        /// <param name="action">Input action.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> action)
            : base(service)
        {
            this.action = action;
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
                this.List();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void List()
        {
            listRecordsInService = this.service.GetRecords();
            this.action(listRecordsInService);
        }
    }
}
