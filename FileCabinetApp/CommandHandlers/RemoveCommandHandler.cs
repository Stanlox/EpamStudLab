using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for deleting a record.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
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

            const string name = "remove";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                Remove(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Remove(string parameters)
        {
            try
            {
                int recordId;
                bool success = int.TryParse(parameters, out recordId);
                if (!success)
                {
                    Console.WriteLine("Conversion error.");
                }

                Program.listRecordsInService = Program.fileCabinetService.GetRecords();
                for (int i = 0; i < Program.listRecordsInService.Count; i++)
                {
                    if (recordId == Program.listRecordsInService[i].Id)
                    {
                        Program.fileCabinetService.RemoveRecord(recordId);
                        Console.WriteLine($"Record #{recordId} is removed");
                        success = true;
                        break;
                    }
                }

                if (!success)
                {
                    throw new ArgumentException($"Record #{recordId} doesn't exists.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
