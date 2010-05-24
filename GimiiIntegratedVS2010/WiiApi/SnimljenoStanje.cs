using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi
{
    class SnimljenoStanje
    {
        public SnimljenoStanje(Stanje stanje, long vreme)
        {
            this.vreme = vreme;
            this.stanje = stanje;
        }

        //Stanje koje je snimljeno
        private Stanje stanje;
        private long vreme;

        public long Vreme
        {
            get { return vreme; }
            set { vreme = value; }
        }

        public Stanje Stanje
        {
            get { return stanje; }
            set { stanje = value; }
        }
    }
}
