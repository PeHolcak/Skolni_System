using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucitelsky_system
{
    sealed class Ucitel:Osoba
    {
        public Ucitel(string jmeno, string prijmeni, DateTime datumNarozeni, int id) : base(jmeno, prijmeni, datumNarozeni, id)
        {

        }
        public Ucitel deepCopy()
        {
            Ucitel novyUcitel =  (Ucitel)this.MemberwiseClone();
            return novyUcitel;
        }
    }
}
