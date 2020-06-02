using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// concrete handler for find a record.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Input service.</param>
        public FindCommandHandler(IFileCabinetService service)
            : base(service)
        {
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

            const string name = "find";
            if (string.Equals(request.Command, name, StringComparison.OrdinalIgnoreCase))
            {
                this.Find(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// override ToString().
        /// </summary>
        /// <returns>string representation of an object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Program.listRecordsInService.Count; i++)
            {
                builder.Append($"{Program.listRecordsInService[i].Id}, ");
                builder.Append($"{Program.listRecordsInService[i].FirstName}, ");
                builder.Append($"{Program.listRecordsInService[i].LastName}, ");
                builder.Append($"{Program.listRecordsInService[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{Program.listRecordsInService[i].Gender}, ");
                builder.Append($"{Program.listRecordsInService[i].Age}, ");
                builder.Append($"{Program.listRecordsInService[i].Salary}");
            }

            return builder.ToString();
        }

        private void Find(string parameters)
        {
            try
            {
                var parameterValue = parameters.Split(' ').Last().Trim('"');
                var parameterArray = parameters.Split(' ');
                var parameterName = parameterArray[parameterArray.Length - 2];
                switch (parameterName.ToLower(CultureInfo.CurrentCulture))
                {
                    case "firstname":
                        Program.listRecordsInService = this.service.FindByFirstName(parameterValue);
                        this.ListRecord(Program.listRecordsInService);
                        break;
                    case "lastname":
                        Program.listRecordsInService = this.service.FindByLastName(parameterValue);
                        this.ListRecord(Program.listRecordsInService);
                        break;
                    case "dateofbirth":
                        Program.listRecordsInService = this.service.FindByDateOfBirth(parameterValue);
                        this.ListRecord(Program.listRecordsInService);
                        break;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// string representation of the collection.
        /// </summary>
        private void ListRecord(ReadOnlyCollection<FileCabinetRecord> listRecordsInService)
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
    }
}
