using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucitelsky_system
{
    class Predmet
    {
        public int id { get; set; }
        public string nazev { get; set; }
        public TimeSpan zacatek { get; set; }
        public int trvani { get; set; }
        public int IdUcitele { get; set; }

        public Predmet(int id, string nazev, TimeSpan zacatek, int trvani, int IdUcitele)
        {
            this.id = id;
            this.nazev = nazev;
            this.zacatek = zacatek;
            this.trvani = trvani;
            this.IdUcitele = IdUcitele;
        }

    }
}
