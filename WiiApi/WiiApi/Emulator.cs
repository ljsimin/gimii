using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WiiApi
{
    class Emulator : Kontroler
    {
        public Emulator(BinaryReader r)
        {

        }

        #region Kontroler Members

        public bool postaviLED(int pozicija, bool ukljucena)
        {
            //Nema operacije ovde
            return true;
        }

        public bool postaviVibrator(bool ukljucen)
        {
            //Nema operacije ovde
            return true;
        }

        public event ObradjivacPromeneStanja PromenaStanja;

        public event ObradjivacOtpustanjaDugmeta OtpustenoDugme;

        public event ObradjivacPritiskaDugmeta PritisnutoDugme;

        public void kreni(bool ponavljanje)
        {
            throw new NotImplementedException();
        }

        public void pauza()
        {
            throw new NotImplementedException();
        }

        public void prekiniKomunikaciju()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
