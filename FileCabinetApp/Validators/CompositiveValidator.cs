using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CompositiveValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        protected CompositiveValidator(IEnumerable<IRecordValidator> validator)
        {
            this.validators = validator.ToList();
        }

        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }
        }
    }
}
