using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for clearing records.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// handles the specified request.
        /// </summary>
        /// <param name="request">Input handle.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"The {nameof(request)} is null");
            }

            const string name = "purge";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                Purge(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Purge(string parameters)
        {
            try
            {
                var tuple = Program.fileCabinetService.PurgeRecord();
                Console.WriteLine($"Data file processing is completed: {tuple.Item1} of {tuple.Item2} records were purged.");
            }
            catch (NotImplementedException)
            {
            }
        }
    }
}
