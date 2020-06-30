using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for exit from application.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private Action<bool> isRunning;
        private FileStream filestream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="isRunning">Input action.</param>
        /// <param name="stream">Input FileStream.</param>
        public ExitCommandHandler(Action<bool> isRunning, FileStream stream)
        {
            this.filestream = stream;
            this.isRunning = isRunning;
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

            const string name = "exit";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Exit();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Exit()
        {
            if (this.filestream != null)
            {
                this.filestream.Dispose();
            }

            Console.WriteLine("Exiting an application...");
            this.isRunning(false);
        }
    }
}
