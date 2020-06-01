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
    public class ListCommandHandler : CommandHandlerBase
    {
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
                List(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// string representation of the collection.
        /// </summary>
        private static void ListRecord(ReadOnlyCollection<FileCabinetRecord> listRecordsInService)
        {
            for (int i = 0; i < listRecordsInService.Count; i++)
            {
                var builder = new StringBuilder();
                builder.Append($"{listRecordsInService[i].Id}, ");
                builder.Append($"{listRecordsInService[i].FirstName}, ");
                builder.Append($"{listRecordsInService[i].LastName}, ");
                builder.Append($"{listRecordsInService[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{listRecordsInService[i].Gender}, ");
                builder.Append($"{listRecordsInService[i].Age}, ");
                builder.Append($"{listRecordsInService[i].Salary}");
                Console.WriteLine("#" + builder.ToString());
            }
        }

        private static void List(string parameters)
        {
            Program.listRecordsInService = Program.fileCabinetService.GetRecords();
            ListRecord(Program.listRecordsInService);
        }
    }
}
