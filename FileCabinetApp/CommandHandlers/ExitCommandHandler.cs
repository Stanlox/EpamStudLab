using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for exit from application.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
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

            const string name = "exit";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                Exit(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Exit(string parameters)
        {
            if (Program.fileStream != null)
            {
                Program.fileStream.Close();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
