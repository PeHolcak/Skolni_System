using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucitelsky_system
{
    sealed class Student : Osoba
    {
        public Student(string jmeno, string prijmeni, DateTime datumNarozeni, int id):base(jmeno, prijmeni, datumNarozeni, id)
        {

        }

    }
}
