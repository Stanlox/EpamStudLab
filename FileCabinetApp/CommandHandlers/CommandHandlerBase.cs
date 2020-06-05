using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// interface implementation.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// fields for setting the following handle.
        /// </summary>
        private ICommandHandler nextHandler;

        /// <summary>
        /// abstract method for executing the query.
        /// </summary>
        /// <param name="request">Input request.</param>
        /// <returns>AppCommandRequest instance.</returns>
        public virtual AppCommandRequest Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                return this.nextHandler.Handle(request);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// set the next handler in the chain.
        /// </summary>
        /// <param name="handler">Input handler.</param>
        /// <returns>Current handler.</returns>
        public ICommandHandler SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
            return handler;
        }
    }
}
