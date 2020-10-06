using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucitelsky_system
{
    abstract class Osoba
    {
        public string Id { get; set; }
        public string jmeno { get; set; }
        public string celeJmeno { get; set; }
        public string prijmeni { get; set; }
        public DateTime datumNarozeni { get; set; }

        public Osoba(string jmeno, string prijmeni, DateTime datumNarozeni, int id)
        {
            this.jmeno = jmeno;
            this.prijmeni = prijmeni;
            this.datumNarozeni = datumNarozeni;
            celeJmeno = String.Format("{0} {1}", jmeno, prijmeni);
            this.Id = id.ToString();
        }

        public string VratCeleJmeno()
        {
            return String.Format("{0} {1}", jmeno, prijmeni);
        }

        public override string ToString()
        {
            return jmeno + " " + prijmeni;
        }
    }
}
