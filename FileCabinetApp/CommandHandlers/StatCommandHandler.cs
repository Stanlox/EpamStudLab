﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for getting statistics.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public StatCommandHandler(IFileCabinetService service)
        {
            this.service = service;
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

            const string name = "stat";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Stat(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Stat(string parameters)
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s).");
            Console.WriteLine($"{recordsCount.Item2} deleted record(s).");
        }
    }
}