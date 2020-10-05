using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Concrete handler print missed command.
    /// </summary>
    public class PrintMissedCommandHandler : CommandHandlerBase
    {
        private static List<string> commands = new List<string>()
        {
            "help",
            "exit",
            "stat",
            "create",
            "export",
            "import",
            "purge",
            "insert",
            "delete",
            "update",
            "select",
        };

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

            PrintMissedCommandInfo(request);
            return null;
        }

        private static void PrintMissedCommandInfo(AppCommandRequest request)
        {
            Console.WriteLine($"There is no '{request.Command}' command.");
            var simalarCommands = GetMostSimilarCommands(request);
            PrintMostSimilarCommands(simalarCommands);
            Console.WriteLine();
        }

        private static void PrintMostSimilarCommands(IEnumerable<string> commands)
        {
            if (commands.Count() == 1)
            {
                Console.WriteLine("The most similar command is:");
                Console.WriteLine("\t{0}", commands.ToArray());
            }
            else if (commands.Count() > 1)
            {
                Console.WriteLine("The most similar commands are:");
                foreach (var command in commands)
                {
                    Console.WriteLine("\t{0}", command);
                }
            }
        }

        private static IEnumerable<string> GetMostSimilarCommands(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestCommandSymbols = request.Command.ToUpperInvariant();
            var commandsIntersactions = commands.Select(command => (command, command.ToUpperInvariant()))
                .Select(commandTuple => (commandTuple.command, commandTuple.Item2.Intersect(requestCommandSymbols).Count()));
            var max = commandsIntersactions.Max(tuple => tuple.Item2);
            return max > 2 ? commandsIntersactions.Where(tuple => tuple.Item2.Equals(max)).Select(tuple => tuple.command) : Enumerable.Empty<string>();
        }
    }
}
