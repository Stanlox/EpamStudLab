namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  defines the service type.
    /// </summary>
    public class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Defines the type of service.
        /// </summary>
        #pragma warning disable CA1051, SA1401
        protected readonly IFileCabinetService service;
        #pragma warning restore CA1051, SA1401

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.service = service;
        }
    }
}
