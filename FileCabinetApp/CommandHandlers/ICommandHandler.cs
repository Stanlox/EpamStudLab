using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        ICommandHandler SetNext(ICommandHandler handler);

        object Handle(AppCommandRequest request);
    }
}
