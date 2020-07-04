using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Consists methods for implementing compositive pattern.
    /// </summary>
    public class CompositiveValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositiveValidator"/> class.
        /// </summary>
        /// <param name="validator">Input all validators.</param>
        public CompositiveValidator(IEnumerable<IRecordValidator> validator)
        {
            this.validators = validator.ToList();
        }

        /// <summary>
        /// Validates all parameters.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        public void ValidateParameters(FileCabinetServiceContext parameters)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }
        }
    }
}
