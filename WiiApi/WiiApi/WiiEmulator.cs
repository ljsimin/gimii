﻿using System;E
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi
{
    class WiiEmulator : Kontroler
    {
        public WiiEmulator(BinaryReadeer r){

        }

        #region Kontroler Members

        public bool postaviLED(int pozicija, bool ukljucena)
        {
            //Nema operacije ovde
            return true;
        }

        public bool ukljuciVibrator(bool ukljucen)
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
