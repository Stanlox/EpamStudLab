using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public char Gender { get; set; }

        public short Age { get; set; }

        public decimal Salary { get; set; }

        public static implicit operator List<object>(FileCabinetRecord v)
        {
            throw new NotImplementedException();
        }
    }
}
