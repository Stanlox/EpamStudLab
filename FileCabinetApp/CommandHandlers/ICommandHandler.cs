using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The Handler interface declares a method for building a chain of handlers. It also declares a method for executing the query.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// set the next handler in the chain.
        /// </summary>
        /// <param name="handler">Input handler.</param>
        /// <returns>Current handler.</returns>
        ICommandHandler SetNext(ICommandHandler handler);

        /// <summary>
        /// method for executing the query.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        AppCommandRequest Handle(AppCommandRequest request);
    }
}
